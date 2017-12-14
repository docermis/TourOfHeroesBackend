using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TourOfHeroesBackend.Models;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace TourOfHeroesBackend.Controllers
{
    [Route( "api/[controller]" )]
    public class DetailController : Controller
    {
        string database = "HeroDatabase.xml";

        // GET api/values
        //Get all heroes
        [HttpGet]
        public IEnumerable<Hero> Get()
        {
            List<Hero> heroList = new List<Hero>();
            XmlDocument doc = new XmlDocument();

            doc.Load( database );

            foreach ( XmlNode node in doc.SelectNodes( "//Heroes/Hero" ) )
            {
                Hero model = new Hero
                {
                    Id = Convert.ToInt32( node.ChildNodes[0].InnerText ),
                    Name = node.ChildNodes[1].InnerText,
                    Power = node.ChildNodes[2].InnerText,
                    Identity = node.ChildNodes[3].InnerText
                };
                heroList.Add( model );
            }

            return heroList;
        }

        // GET api/values/5
        //Get hero with specific id
        [HttpGet( "{id}" )]
        public Hero Get( int id )
        {
            Hero hero = new Hero();
            XmlDocument doc = new XmlDocument();

            doc.Load( database );

            foreach ( XmlNode node in doc.SelectNodes( "//Heroes/Hero" ) )
            {
                if ( id == Convert.ToInt32( node.ChildNodes[0].InnerText ) )
                {
                    hero.Id = Convert.ToInt32( node.ChildNodes[0].InnerText );
                    hero.Name = node.ChildNodes[1].InnerText;
                    hero.Power = node.ChildNodes[2].InnerText;
                    hero.Identity = node.ChildNodes[3].InnerText;
                    break;
                }
            }

            return hero;
        }

        // GET api/values/byName/superman
        //Get hero with specific name
        [HttpGet( "byName/{name}" )]
        public Hero Get( string name )
        {
            Hero hero = new Hero();
            XmlDocument doc = new XmlDocument();

            doc.Load( database );

            foreach ( XmlNode node in doc.SelectNodes( "//Heroes/Hero" ) )
            {
                if ( name.Equals( node.ChildNodes[1].InnerText ) )
                {
                    hero.Id = Convert.ToInt32( node.ChildNodes[0].InnerText );
                    hero.Name = node.ChildNodes[1].InnerText;
                    hero.Power = node.ChildNodes[2].InnerText;
                    hero.Identity = node.ChildNodes[3].InnerText;
                    break;
                }
            }

            return hero;
        }





        // POST api/values
        [HttpPost]
        public Hero Post( [FromBody]Hero hero )
        {
            int id, count = 1;
            XmlDocument doc = new XmlDocument();
            doc.Load( database );




            XmlNode heroesNode = doc.SelectSingleNode( "//Heroes" );

            foreach ( XmlNode nodeSearch in doc.SelectNodes( "//Heroes/Hero" ) )
            {
                if ( Convert.ToInt32( nodeSearch.ChildNodes[0].InnerText ) > count )
                {
                    break;
                }
                count++;
            }
            id = count;

            XmlNode heroRef = doc.SelectSingleNode( "//Heroes" ).ChildNodes[count - 1];

            XmlNode heroNode = doc.CreateElement( "Hero" );


            XmlNode node = doc.CreateElement( "id" );
            node.InnerText = ( id ).ToString();
            heroNode.AppendChild( node );
            hero.Id = id;

            node = doc.CreateElement( "name" );
            node.InnerText = hero.Name;
            heroNode.AppendChild( node );

            node = doc.CreateElement( "power" );
            node.InnerText = hero.Power;
            heroNode.AppendChild( node );

            node = doc.CreateElement( "identity" );
            node.InnerText = hero.Identity;
            heroNode.AppendChild( node );

            heroesNode.InsertBefore( heroNode, heroRef );

            doc.Save( database );

            return hero;
        }

        // PUT api/values/5
        [HttpPut( "{id}" )]
        public void Put( int id, [FromBody]Hero hero )
        {
            XmlDocument doc = new XmlDocument();
            doc.Load( database );

            foreach ( XmlNode node in doc.SelectNodes( "//Heroes/Hero" ) )
            {
                if ( id == Convert.ToInt32( node.ChildNodes[0].InnerText ) )
                {
                    node.ChildNodes[0].InnerText = hero.Id.ToString();
                    node.ChildNodes[1].InnerText = hero.Name;
                    node.ChildNodes[2].InnerText = hero.Power;
                    node.ChildNodes[3].InnerText = hero.Identity;
                    break;
                }
            }

            doc.Save( database );
        }


        // DELETE api/values/5
        [HttpDelete( "{id}" )]
        public void Delete( int id )
        {
            XmlDocument doc = new XmlDocument();
            doc.Load( database );

            XmlNode parent = doc.SelectSingleNode( "//Heroes" );


            foreach ( XmlNode node in parent.ChildNodes )
            {
                if ( id == Convert.ToInt32( node.ChildNodes[0].InnerText ) )
                {
                    node.RemoveAll();
                    parent.RemoveChild( node );
                    break;
                }
            }
            doc.Save( database );

        }
    }
}
