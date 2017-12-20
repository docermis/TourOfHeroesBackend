using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using TourOfHeroesBackend.Models;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TourOfHeroesBackend.Controllers
{
    [Route("api/[controller]")]
    public class FileController : Controller
    {
        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/file/term
        [HttpGet("{searchTerm}")]
        public string[] Get(string searchTerm)
        {
            string[] fileNamesList = Directory.GetFiles( @"C:\Users\eioannidis\source\repos\MyDatabase\", "*" + searchTerm + "*" );



            return fileNamesList;
        }
        

        // POST api/file
        [HttpPost]
        public Info UploadFile( IFormFile file )
        {
            Info info = new Info();
            Random num = new Random( DateTime.Now.Millisecond );
            int count = num.Next();
            if ( file == null )
            {
                throw new Exception( "File is null." );
            }
            if ( file.Length == 0 )
            {
                throw new Exception( "File is empty." );
            }
            int period = file.FileName.IndexOf( "." );
            string ext = file.FileName.Substring( period );
            string dirPath = @"C:\Users\eioannidis\source\repos\MyDatabase\";
            string fileName = string.Format( @"File{0}{1}", count, ext );    //file.FileName;    //tha tou dinw egw unique name gia twra pou den iparxei vasi.
            string targetPath = System.IO.Path.Combine( dirPath, fileName );
            string xmlFileName = string.Format( @"File{0}{1}.xml", count, ext );
            string xmlTargetPath = System.IO.Path.Combine( dirPath, xmlFileName );
            info.XmlFileName = xmlFileName;

            if ( !System.IO.File.Exists( targetPath ) )
            {

                //na tsekarw an uparxei idi to arxeio
                using ( Stream stream = file.OpenReadStream() )
                using ( Stream targetStream = System.IO.File.OpenWrite( targetPath ) )
                {
                    using ( BinaryReader binaryReader = new BinaryReader( stream ) )
                    using ( BinaryWriter binaryWriter = new BinaryWriter( targetStream ) )
                    {
                        var fileContent = binaryReader.ReadBytes( (int) file.Length );
                        binaryWriter.Write( fileContent );
                    }
                }

                if ( !System.IO.File.Exists( xmlTargetPath ) )
                {
                    using ( FileStream fs = System.IO.File.Create( xmlTargetPath ) )
                    {

                        XmlDocument doc = new XmlDocument();

                        //Create an XML declaration. 
                        XmlDeclaration xmldecl = doc.CreateXmlDeclaration( "1.0", "utf-8", null );
                        //Create the XML root Element
                        XmlElement root = doc.CreateElement( "File" );
                        root.SetAttribute( "lang", "en" );
                        //Append the new nodes to the document.
                        doc.AppendChild( root );
                        doc.InsertBefore( xmldecl, root );

                        //Create node for origianl name, my name, creation date and append it to root
                        XmlNode node = doc.CreateElement( "OriginalName" );
                        node.InnerText = file.FileName;
                        root.AppendChild( node );
                        node = doc.CreateElement( "MyName" );
                        node.InnerText = fileName;
                        root.AppendChild( node );
                        node = doc.CreateElement( "CreationDate" );
                        node.InnerText = DateTime.Now.ToString();
                        root.AppendChild( node );

                        doc.Save( fs );



                    }
                }

            }
            else
            {
                throw new Exception( "File already exists." );
            }

            return info;
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
