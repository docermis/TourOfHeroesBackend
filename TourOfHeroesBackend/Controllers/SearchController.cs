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



namespace TourOfHeroesBackend.Controllers
{

    [Route( "api/[controller]" )]
    public class SearchController : Controller
    {
        string database = "HeroDatabase.xml";

        // GET: api/search
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/search/superman
        [HttpGet( "{term}" )]
        public IEnumerable<Hero> Get( string term )
        {
            List<Hero> heroList = new List<Hero>();
            XmlDocument doc = new XmlDocument();

            doc.Load( database );

            foreach ( XmlNode node in doc.SelectNodes( "//Heroes/Hero" ) )
            {
                Hero hero = new Hero();
                if ( node.ChildNodes[1].InnerText.IndexOf( term, StringComparison.OrdinalIgnoreCase ) >= 0 )
                {
                    hero.Id = Convert.ToInt32( node.ChildNodes[0].InnerText );
                    hero.Name = node.ChildNodes[1].InnerText;
                    hero.Power = node.ChildNodes[2].InnerText;
                    hero.Identity = node.ChildNodes[3].InnerText;
                    heroList.Add( hero );
                }
            }

            return heroList;
        }
        
        

        // POST api/values
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

        [Route( "form" )]
        [HttpPost]
        public void UpdateXmlFile([FromBody] Info fileInfo )
        {
            string dirPath = @"C:\Users\eioannidis\source\repos\MyDatabase\";
            string xmlTargetPath = System.IO.Path.Combine( dirPath, fileInfo.XmlFileName );
            XmlDocument doc = new XmlDocument();
            doc.Load( xmlTargetPath );

            XmlNode root = doc.SelectSingleNode( "//File" );

            XmlElement element = doc.CreateElement( "Title" );
            element.InnerText = fileInfo.Title;
            root.AppendChild( element );
            element = doc.CreateElement( "UploaderName" );
            element.InnerText = fileInfo.UploaderName;
            root.AppendChild( element );
            element = doc.CreateElement( "Description" );
            element.InnerText = fileInfo.Description;
            root.AppendChild( element );

            doc.Save( xmlTargetPath );
        }

        // PUT api/values/5
        [HttpPut( "{id}" )]
        public void Put( int id, [FromBody]string value )
        {
        }

        // DELETE api/values/5
        [HttpDelete( "{id}" )]
        public void Delete( int id )
        {
        }
    }
}
