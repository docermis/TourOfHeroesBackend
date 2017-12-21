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
    [Route( "api/[controller]" )]
    public class FileController : Controller
    {
        string dirPath = @"C:\Users\eioannidis\source\repos\MyDatabase\";


        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        //GET api/file/download/id
        [HttpGet( "download/{id}" )]
        public FileResult Get( int id )
        {
            string targetFileName = "";
            string targetRealFileName = "";
            XmlDocument doc = new XmlDocument();
            doc.Load( dirPath + "MyDatabaseData.xml" );

            XmlNode databaseNode = doc.SelectSingleNode( "//Database" );

            foreach ( XmlNode node in databaseNode )
            {
                if ( node.ChildNodes[0].InnerText == id.ToString() )
                {
                    targetFileName = node.ChildNodes[2].InnerText;
                    targetRealFileName = node.ChildNodes[1].InnerText;
                    break;
                }
            }
            string filePath = System.IO.Path.Combine( dirPath, targetFileName );

            System.IO.FileStream fs = System.IO.File.OpenRead( filePath );
            //BinaryReader br = new BinaryReader( fs );
            //var fileContent = br.ReadBytes( (int) fs.Length );

            return File( fs, System.Net.Mime.MediaTypeNames.Application.Octet, targetRealFileName );
            // return File(fileContent, System.Net.Mime.MediaTypeNames.Application.Octet, targetRealFileName );
        }

        // GET api/file/term
        [HttpGet( "{searchTerm}" )]
        public IEnumerable<MyDatabaseData> Get( string searchTerm )
        {
            List<MyDatabaseData> fileDataList = new List<MyDatabaseData>();

            XmlDocument doc = new XmlDocument();
            doc.Load( dirPath + "MyDatabaseData.xml" );

            XmlNode databaseNode = doc.SelectSingleNode( "//Database" );

            foreach ( XmlNode node in databaseNode )
            {
                MyDatabaseData myDatabaseData = new MyDatabaseData();
                if ( node.ChildNodes[1].InnerText.IndexOf( searchTerm, StringComparison.OrdinalIgnoreCase ) >= 0 )
                {
                    myDatabaseData.Id = Convert.ToInt32( node.ChildNodes[0].InnerText );
                    myDatabaseData.OriginalFileName = node.ChildNodes[1].InnerText;
                    myDatabaseData.MyFileName = node.ChildNodes[2].InnerText;
                    fileDataList.Add( myDatabaseData );
                }
            }


            //if ( !(searchTerm != null) )
            //{
            //    foreach ( XmlNode node in databaseNode )
            //    {
            //        MyDatabaseData myDatabaseData = new MyDatabaseData();
            //        myDatabaseData.Id = Convert.ToInt32( node.ChildNodes[0].InnerText );
            //        myDatabaseData.OriginalFileName = node.ChildNodes[1].InnerText;
            //        myDatabaseData.MyFileName = node.ChildNodes[2].InnerText;
            //        fileDataList.Add( myDatabaseData );
            //    }
            //}

            return fileDataList;
        }


        // POST api/file
        [HttpPost]
        public Info UploadFile( IFormFile file )
        {
            Info info = new Info();


            if ( file == null )
            {
                throw new Exception( "File is null." );
            }
            if ( file.Length == 0 )
            {
                throw new Exception( "File is empty." );
            }

            string ext = "";
            foreach ( char c in file.FileName )
            {
                if ( c.Equals( '.' ) )
                {
                    ext = ".";
                }
                else
                {
                    ext += c;
                }

            }

            XmlDocument doc = new XmlDocument();
            doc.Load( dirPath + "MyDatabaseData.xml" );
            int id;
            int count = 1;

            XmlNode databaseNode = doc.SelectSingleNode( "//Database" );
            foreach ( XmlNode temp in databaseNode )
            {
                if ( count < Convert.ToInt32( temp.ChildNodes[0].InnerText ) )
                {
                    break;
                }
                count++;
            }
            id = count;

            string fileName = string.Format( @"File{0}{1}", id, ext );      //tha tou dinw egw unique name gia twra pou den iparxei vasi.
            string targetPath = System.IO.Path.Combine( dirPath, fileName );
            string xmlFileName = string.Format( @"File{0}{1}.xml", id, ext );
            string xmlTargetPath = System.IO.Path.Combine( dirPath, xmlFileName );
            info.XmlFileName = xmlFileName;

            if ( !System.IO.File.Exists( targetPath ) )
            {

                XmlNode fileNode = doc.CreateElement( "File" );
                databaseNode.AppendChild( fileNode );

                XmlNode node = doc.CreateElement( "Id" );
                node.InnerText = id.ToString();
                fileNode.AppendChild( node );
                node = doc.CreateElement( "OriginalName" );
                node.InnerText = file.FileName;
                fileNode.AppendChild( node );
                node = doc.CreateElement( "MyName" );
                node.InnerText = fileName;
                fileNode.AppendChild( node );

                doc.Save( dirPath + "MyDatabaseData.xml" );


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

                        doc = new XmlDocument();

                        //Create an XML declaration. 
                        XmlDeclaration xmldecl = doc.CreateXmlDeclaration( "1.0", "utf-8", null );
                        //Create the XML root Element
                        XmlElement root = doc.CreateElement( "File" );
                        root.SetAttribute( "lang", "en" );
                        //Append the new nodes to the document.
                        doc.AppendChild( root );
                        doc.InsertBefore( xmldecl, root );

                        //Create node for origianl name, my name, creation date and append it to root
                        node = doc.CreateElement( "OriginalName" );
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
        [HttpPut( "{id}" )]
        public void Put( int id, [FromBody]string value )
        {
        }

        // DELETE api/file/
        [HttpDelete( "{id}" )]
        public void Delete( int id )
        {
            MyDatabaseData myDatabaseData = new MyDatabaseData();
            XmlDocument doc = new XmlDocument();
            doc.Load( dirPath + "MyDatabaseData.xml" );
            XmlNode root = doc.SelectSingleNode( "//Database" );

            foreach ( XmlNode node in root )
            {
                if ( Convert.ToInt32( node.ChildNodes[0].InnerText ) == id )
                {
                    myDatabaseData.Id = Convert.ToInt32( node.ChildNodes[0].InnerText );
                    myDatabaseData.OriginalFileName = node.ChildNodes[1].InnerText;
                    myDatabaseData.MyFileName = node.ChildNodes[2].InnerText;
                    node.RemoveAll();
                    root.RemoveChild( node );
                    break;
                }
            }
            doc.Save( dirPath + "MyDatabaseData.xml" );

            string targetFilePath = System.IO.Path.Combine( dirPath, myDatabaseData.MyFileName );
            string targetXmlFilePath = System.IO.Path.Combine( dirPath, string.Format( @"{0}.xml", myDatabaseData.MyFileName ) );

            System.IO.File.Delete( targetFilePath );
            System.IO.File.Delete( targetXmlFilePath );

        }
    }
}
