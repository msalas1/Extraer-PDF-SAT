using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text.RegularExpressions;
using System.Text;
using System.Data;


namespace PDFExtraer
{

    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        string outPath = @"C:\Users\IntegraCorp\Desktop\test1.txt";
        
        protected void btnGenerar_Click(object sender, EventArgs e)
        {
            //Verifica si ya hay un archivo de salida y lo elimina:
            if (File.Exists(outPath))
            {
                File.Delete(outPath);
            }

            Check();
            if (Activado && fuPDFS.HasFile)
            {
                UploadFiles();
                pdfDirectory();

                if (File.Exists(outPath))
                {
                    using (TextReader tr = new StreamReader(outPath))
                    {
                        ConvertToDataTable(outPath);
                        
                        if (skip.Count()>0)
                        {
                            lblShow.Text = "Se creó el archivo test1.txt en carpeta C:/" + "<br/>" + 
                                            "No se pudieron extrar los siguientes archivos: " + "<br/>" +
                                            string.Join("<br />", skip); 
                        }
                        else
                        {
                            lblShow.Text = "Se creó el archivo test1.txt en carpeta C:/";
                        }
                        
                    }
                    btnGenerar.Enabled = false;
                    fuPDFS.Enabled = false;
                }
                else
                {
                    lblShow.Text = "No hay ningún archivo para mostrar.";
                }
            }
            else
            {
                
                Response.Write("<script>alert('No seleccionó ningun dato a extraer.'); window.location='Default.aspx'</script>");

            }

        }

        public void pdfDirectory()
        {
            string[] filePaths = Directory.GetFiles(Server.MapPath(@"~/UploadedPDFs/"), "*.pdf");
            foreach (string fp in filePaths)
            {
                ExtractTextFromPdf(fp);
            }
        }

        List<string> skip = new List<string>();
        public void ExtractTextFromPdf(string path)
        {
            //outPath = C:\test1.txt
            //extraer lineas de pdf
            string output = string.Empty;
            using (PdfReader reader = new PdfReader(path))
            {
                StringBuilder text = new StringBuilder();

                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                }
                string lines = text.ToString();
                
                //remplazar espacios y saltos de línea
                lines = lines.Replace(" ", "#");
                lines = lines.Replace("\n", "#");
                lines = lines.Replace("(s)", "###"); //se añadió para que la funcion pueda leer "(s)"


                //Validaciones del check
                List<string> ValuesChecked = new List<string>();
                CheckBox[] checkboxes = { chRFC, chCP, chCURP, chName, chSTREET };
                foreach (CheckBox checkbox in checkboxes)
                {
                    if (checkbox.Checked == true)
                    {
                        ValuesChecked.Add(checkbox.Attributes["Value"]);
                    }
                }
                string[] stringToCheck = { "RFC", "CURP", "NAME", "CP", "STREET" };
                try
                {
                    if (ValuesChecked.Any(stringToCheck.Contains))
                    {
                        inCheck = ValuesChecked;


                        ///validación para dos nombres:
                        //números de index (irán cambiando sus posiciones dependiendo de que otros datos querramos extraer)
                        //RFC(1),
                        //CURP(2),
                        //NOMBRE(3)
                        //P.Apellido(4),
                        //S.Apellido (5),
                        //CP(6).
                        //NombreVialidad(7)
                        //NumeroExterior(8)
                        var extract = Regex.Match(lines, @"#del#Contribuyente:##(.*?)#RFC:#CURP:#(.*?)#Nombre####:#(.*).*#Primer#Apellido:#(.*?)#Segundo#Apellido:#(.*?)#Fecha#inicio.*#registrado##Código#Postal:(.*?)#Tipo#.*#Nombre#de#Vialidad:#(.*).*#Número#Exterior:#(.*?)#Número#Interior:#.");
                        if (extract.Success == false)
                        {
                            extract = Regex.Match(lines, @"#del#Contribuyente:##RFC:#(.*?)#CURP:#(.*?)#Nombre####:#(.*).*#Primer#Apellido:#(.*?)#(.*?)#Segundo#Apellido:#Fecha#inicio.*#registrado##Código#Postal:(.*?)#Tipo#.*#Nombre#de#Vialidad:#(.*).*#Número#Exterior:#(.*?)#Número#Interior:#.");
                            if (extract.Success == false)
                            {
                                extract = Regex.Match(lines, @"#del#Contribuyente:##RFC:#(.*?)#CURP:#(.*?)#(.*).*#Nombre####:#Primer#Apellido:#(.*?)#Segundo#Apellido:#(.*?)#Fecha#inicio.*#registrado##Código#Postal:(.*?)#Tipo#.*#Nombre#de#Vialidad:#(.*).*#Número#Exterior:#(.*?)#Número#Interior:#.");
                                if (extract.Success == false)
                                {
                                    extract = Regex.Match(lines, @"#del#Contribuyente:##RFC:#(.*?)#CURP:#(.*?)#Nombre####:#(.*).*#Primer#Apellido:#(.*?)#(.*?)#Segundo#Apellido:#Fecha#inicio.*#registrado##Código#Postal:(.*?)#Tipo#.*#Nombre#de#Vialidad:#(.*).*#Número#Exterior:#(.*?)#Número#Interior:.*#");
                                    if (extract.Success == false)
                                    {
                                        skip.Add(System.IO.Path.GetFileName(path));
                                    }
                                    else
                                    {
                                        if (ValuesChecked.Contains("RFC"))
                                        {
                                            output += string.Concat(extract.Groups[1] + "|");

                                        }
                                        if (ValuesChecked.Contains("CURP"))
                                        {
                                            output += string.Concat(extract.Groups[2] + "|");
                                        }
                                        if (ValuesChecked.Contains("NAME"))
                                        {
                                            output += string.Concat(
                                                 extract.Groups[3] + "|" + //NAME1
                                                                           //extract.Groups[4] + "|" + //NAME2
                                                 extract.Groups[4] + "|" + //P.APELLIDO
                                                 extract.Groups[5] + "|"   //S.APELLIDO
                                                 );
                                            output = output.Replace("#", " ");
                                            ValuesChecked.Add("lastname");
                                            ValuesChecked.Add("s_lastname");
                                        }
                                        if (ValuesChecked.Contains("CP"))
                                        {
                                            output += string.Concat(extract.Groups[6] + "|");
                                        }
                                        if (ValuesChecked.Contains("STREET"))
                                        {
                                            output += string.Concat(
                                                extract.Groups[7] + "|" +//NOMBRE VIALIDAD
                                                extract.Groups[8] + "|"); //N.EXTERIOR

                                            output = output.Replace("#", " ");
                                            ValuesChecked.Add("ext_number");
                                        }

                                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(outPath, true))
                                        {

                                            file.WriteLine(output);
                                        }
                                    }
                                    
                                }
                                else
                                {
                                    if (ValuesChecked.Contains("RFC"))
                                    {
                                        output += string.Concat(extract.Groups[1] + "|");

                                    }
                                    if (ValuesChecked.Contains("CURP"))
                                    {
                                        output += string.Concat(extract.Groups[2] + "|");
                                    }
                                    if (ValuesChecked.Contains("NAME"))
                                    {
                                        output += string.Concat(
                                             extract.Groups[3] + "|" + //NAME1
                                                                       //extract.Groups[4] + "|" + //NAME2
                                             extract.Groups[4] + "|" + //P.APELLIDO
                                             extract.Groups[5] + "|"   //S.APELLIDO
                                             );
                                        output = output.Replace("#", " ");
                                        ValuesChecked.Add("lastname");
                                        ValuesChecked.Add("s_lastname");
                                    }
                                    if (ValuesChecked.Contains("CP"))
                                    {
                                        output += string.Concat(extract.Groups[6] + "|");
                                    }
                                    if (ValuesChecked.Contains("STREET"))
                                    {
                                        output += string.Concat(
                                            extract.Groups[7] + "|" +//NOMBRE VIALIDAD
                                            extract.Groups[8] + "|"); //N.EXTERIOR

                                        output = output.Replace("#", " ");
                                        ValuesChecked.Add("ext_number");
                                    }

                                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(outPath, true))
                                    {

                                        file.WriteLine(output);
                                    }
                                }
                                
                            }
                            else
                            {
                                if (ValuesChecked.Contains("RFC"))
                                {
                                    output += string.Concat(extract.Groups[1] + "|");

                                }
                                if (ValuesChecked.Contains("CURP"))
                                {
                                    output += string.Concat(extract.Groups[2] + "|");
                                }
                                if (ValuesChecked.Contains("NAME"))
                                {
                                    output += string.Concat(
                                         extract.Groups[3] + "|" + //NAME1
                                                                   //extract.Groups[4] + "|" + //NAME2
                                         extract.Groups[4] + "|" + //P.APELLIDO
                                         extract.Groups[5] + "|"   //S.APELLIDO
                                         );
                                    output = output.Replace("#", " ");
                                    ValuesChecked.Add("lastname");
                                    ValuesChecked.Add("s_lastname");
                                }
                                if (ValuesChecked.Contains("CP"))
                                {
                                    output += string.Concat(extract.Groups[6] + "|");
                                }
                                if (ValuesChecked.Contains("STREET"))
                                {
                                    output += string.Concat(
                                        extract.Groups[7] + "|" +//NOMBRE VIALIDAD
                                        extract.Groups[8] + "|"); //N.EXTERIOR

                                    output = output.Replace("#", " ");
                                    ValuesChecked.Add("ext_number");
                                }

                                using (System.IO.StreamWriter file = new System.IO.StreamWriter(outPath, true))
                                {

                                    file.WriteLine(output);
                                }
                            }
                        }
                       
                        else if (extract.Success)
                        {
                         
                            if (ValuesChecked.Contains("RFC"))
                            {
                                output += string.Concat(extract.Groups[1] + "|");
                               
                            }
                            if (ValuesChecked.Contains("CURP"))
                            {
                                output += string.Concat(extract.Groups[2] + "|");
                            }
                            if (ValuesChecked.Contains("NAME"))
                            {
                                output += string.Concat(
                                     extract.Groups[3] + "|" + //NAME1
                                     //extract.Groups[4] + "|" + //NAME2
                                     extract.Groups[4] + "|" + //P.APELLIDO
                                     extract.Groups[5] + "|"   //S.APELLIDO
                                     );
                                output = output.Replace("#", " ");
                                ValuesChecked.Add("lastname");
                                ValuesChecked.Add("s_lastname");
                            }
                            if (ValuesChecked.Contains("CP"))
                            {
                                output += string.Concat(extract.Groups[6] + "|");
                            }
                            if (ValuesChecked.Contains("STREET"))
                            {
                                output += string.Concat(
                                    extract.Groups[7] + "|" +//NOMBRE VIALIDAD
                                    extract.Groups[8] + "|"); //N.EXTERIOR

                                output = output.Replace("#", " ");
                                ValuesChecked.Add("ext_number");
                            }

                            using (System.IO.StreamWriter file = new System.IO.StreamWriter(outPath, true))
                            {

                                file.WriteLine(output);
                            }
                        }

                    }
                }
                catch (Exception)
                {
                   
                    Response.Write("<script>alert('Fallo.');</script>");
                    // Response.Redirect(Request.Url.AbsoluteUri);
                    throw;
                }
            }
        }
        public List<string> inCheck { get; private set; }
        public DataTable ConvertToDataTable(string filePath)
        {
            DataTable dt = new DataTable();

            //Validaciones para activar columnas

            dt.Columns.AddRange(new DataColumn[inCheck.Count()]);

           ///Ifs en orden a cómo se construye la cadena del TXT
            if (inCheck.Contains("RFC"))
            {
                dt.Columns.Add(new DataColumn("RFC", typeof(string)));
            }
            else
            {
                GridView1.Columns[3].Visible = false;
            }
            if (inCheck.Contains("CURP"))
            {
                dt.Columns.Add(new DataColumn("CURP", typeof(string)));
            }
            else
            {
                GridView1.Columns[4].Visible = false;
            }
            if (inCheck.Contains("NAME"))
            {
                dt.Columns.Add(new DataColumn("NAME", typeof(string)));
                dt.Columns.Add(new DataColumn("LASTNAME", typeof(string)));
                dt.Columns.Add(new DataColumn("SECONDLASTNAME", typeof(string)));
            }
            else
            {
                GridView1.Columns[0].Visible = false;
                GridView1.Columns[1].Visible = false;
                GridView1.Columns[2].Visible = false;
            }
            
            if (inCheck.Contains("CP"))
            {
                dt.Columns.Add(new DataColumn("CP", typeof(string)));
            }
            else
            {
                GridView1.Columns[5].Visible = false;
            }
            if (inCheck.Contains("STREET"))
            {
                dt.Columns.Add(new DataColumn("STREET", typeof(string)));
                dt.Columns.Add(new DataColumn("EXTNUMBER", typeof(string)));
            }
            else
            {
                GridView1.Columns[6].Visible = false;
                GridView1.Columns[7].Visible = false;
            }

            dt.Columns.Add();



            //separando y añadiendo a la fila cada valor del TXT
            using (System.IO.StreamReader sr = new System.IO.StreamReader(filePath))
            {
                string currentline = string.Empty;
                while ((currentline = sr.ReadLine()) != null)
                {
                    dt.Rows.Add();
                    int colCount = 0;
                    foreach (string item in currentline.Split('|'))
                    {
                        dt.Rows[dt.Rows.Count - 1][colCount] = item;
                        colCount++;
                    }
                }
                GridView1.DataSource = dt;
                GridView1.DataBind();
                return dt;
            }
        }


        //Verificar los checkbox seleccionado, devuelve el valor "Actvado"
        bool Activado = false;
        public void Check()
        {
            CheckBox[] checkboxes = { chRFC, chCP, chCURP, chName, chSTREET };


            foreach (CheckBox checkbox in checkboxes)
            {
                if (checkbox.Checked == true)
                {
                    Activado = true;

                }
            }

        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            Response.Redirect(Request.Url.AbsoluteUri);
        }

        public void UploadFiles() 
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/UploadedPDFs/"));
            if (di.Exists)
            {
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }

            foreach (HttpPostedFile postedFile in fuPDFS.PostedFiles)
            {
                string fileName = System.IO.Path.GetFileName(postedFile.FileName);
                postedFile.SaveAs(Server.MapPath("~/UploadedPDFs/") + fileName);
            }
            lblResult.Text = string.Format("{0} archivo(s) se han cargado correctamente.", fuPDFS.PostedFiles.Count);
            btnGenerar.Enabled = true;
        }
    }

}