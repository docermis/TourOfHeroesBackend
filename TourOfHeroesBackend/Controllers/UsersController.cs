using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Xml;
using TourOfHeroesBackend.Models;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TourOfHeroesBackend.Controllers
{
    [Authorize]
    [Route( "api/[controller]" )]
    public class UsersController : Controller
    {
        string database = "UserDatabase.xml";
        //private IConfiguration _config;

        //public UsersController( IConfiguration config )
        //{
        //    _config = config;
        //}

        private Jwt _config;
        public UsersController( IOptions<Jwt> jwt )
        {
            _config = jwt.Value;
            // _settings.StringSetting == "My Value";
        }



        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<UserDto> Get()
        {
            List<UserDto> userList = new List<UserDto>();
            XmlDocument doc = new XmlDocument();

            doc.Load( database );

            foreach ( XmlNode node in doc.SelectNodes( "//Users/User" ) )
            {
                UserDto model = new UserDto
                {
                    Id = Convert.ToInt32( node.ChildNodes[0].InnerText ),
                    FirstName = node.ChildNodes[1].InnerText,
                    LastName = node.ChildNodes[2].InnerText,
                    Username = node.ChildNodes[3].InnerText,
                    Password = node.ChildNodes[4].InnerText
                };
                userList.Add( model );
            }

            return userList;
        }

        // GET api/<controller>/5
        [HttpGet( "{id}" )]
        public UserDto Get( int id )
        {
            UserDto userDto = new UserDto();
            XmlDocument doc = new XmlDocument();

            doc.Load( database );

            foreach ( XmlNode node in doc.SelectNodes( "//Users/User" ) )
            {
                if ( Convert.ToInt32( node.ChildNodes[0].InnerText ) == id )
                {
                    userDto.Id = Convert.ToInt32( node.ChildNodes[0].InnerText );
                    userDto.FirstName = node.ChildNodes[1].InnerText;
                    userDto.LastName = node.ChildNodes[2].InnerText;
                    userDto.Username = node.ChildNodes[3].InnerText;
                }
            }

            return userDto;
        }

        [HttpGet( "search/{term}" )]
        public IEnumerable<UserDto> Get( string term )
        {
            List<UserDto> userDtoList = new List<UserDto>();
            XmlDocument doc = new XmlDocument();

            doc.Load( database );

            foreach ( XmlNode node in doc.SelectNodes( "//Users/User" ) )
            {
                UserDto userDto = new UserDto();
                if ( node.ChildNodes[1].InnerText.IndexOf( term, StringComparison.OrdinalIgnoreCase ) >= 0 ||
                    node.ChildNodes[2].InnerText.IndexOf( term, StringComparison.OrdinalIgnoreCase ) >= 0 )
                {
                    userDto.Id = Convert.ToInt32( node.ChildNodes[0].InnerText );
                    userDto.FirstName = node.ChildNodes[1].InnerText;
                    userDto.LastName = node.ChildNodes[2].InnerText;
                    userDto.Username = node.ChildNodes[3].InnerText;
                    userDtoList.Add( userDto );
                }
            }

            return userDtoList;
        }

        // POST api/<controller>/register
        [AllowAnonymous]
        [HttpPost( "register" )]
        public UserDto Post( [FromBody]UserDto user )
        {
            int id, count = 1;
            XmlDocument doc = new XmlDocument();
            doc.Load( database );




            XmlNode usersNode = doc.SelectSingleNode( "//Users" );
            foreach ( XmlNode nodeSearch in usersNode )
            {
                if ( Convert.ToInt32( nodeSearch.ChildNodes[0].InnerText ) > count )
                {
                    break;
                }
                count++;
            }
            id = count;
            user.Id = id;

            XmlNode userRef = doc.SelectSingleNode( "//Users" ).ChildNodes[count - 1];

            XmlNode userNode = doc.CreateElement( "User" );


            XmlNode node = doc.CreateElement( "id" );
            node.InnerText = ( id ).ToString();
            userNode.AppendChild( node );

            node = doc.CreateElement( "firstName" );
            node.InnerText = user.FirstName;
            userNode.AppendChild( node );

            node = doc.CreateElement( "lastName" );
            node.InnerText = user.LastName;
            userNode.AppendChild( node );

            node = doc.CreateElement( "username" );
            node.InnerText = user.Username;
            userNode.AppendChild( node );

            node = doc.CreateElement( "password" );
            node.InnerText = user.Password;
            userNode.AppendChild( node );

            usersNode.InsertBefore( userNode, userRef );

            doc.Save( database );

            return user;
        }

        [AllowAnonymous]
        [HttpPost( "authenticate" )]
        public IActionResult Authenticate( [FromBody] UserCredentials userCredentials )
        {
            if ( string.IsNullOrEmpty( userCredentials.Username ) || string.IsNullOrEmpty( userCredentials.Password ) )
                return Unauthorized();

            UserDto userDto = new UserDto
            {
                Id = 0
            };
            XmlDocument doc = new XmlDocument();

            doc.Load( database );

            foreach ( XmlNode node in doc.SelectNodes( "//Users/User" ) )
            {
                if ( node.ChildNodes[3].InnerText == userCredentials.Username )
                {
                    if ( node.ChildNodes[4].InnerText == userCredentials.Password )
                    {
                        //create and return user and token
                        userDto.Id = Convert.ToInt32( node.ChildNodes[0].InnerText );
                        userDto.FirstName = node.ChildNodes[1].InnerText;
                        userDto.LastName = node.ChildNodes[2].InnerText;
                        break;
                    }
                }
            }

            // an den vreis user me to username tote epestrepse oti den uparxei.    
            if ( userDto.Id == 0 )
            {
                return Unauthorized();
            }

            var key = new SymmetricSecurityKey( Encoding.UTF8.GetBytes( _config.Key ) );
            var creds = new SigningCredentials( key, SecurityAlgorithms.HmacSha256 );

            var token = new JwtSecurityToken(
                _config.Issuer,
                _config.Audience,
                expires: DateTime.Now.AddMinutes( 30 ),
                signingCredentials: creds
                );
            
            var tokenString = new JwtSecurityTokenHandler().WriteToken( token );
            //grapse se xml to username kai to token tou xristi pou ekane log in

            return Ok( new
            {
                Id = userDto.Id,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Username = userCredentials.Username,
                Token = tokenString
            } );
        }




        // PUT api/<controller>/5
        [HttpPut( "{id}" )]
        public void Put( int id, [FromBody]UserDto userDto )
        {
            XmlDocument doc = new XmlDocument();
            doc.Load( database );

            foreach ( XmlNode node in doc.SelectNodes( "//Users/User" ) )
            {
                if ( id == Convert.ToInt32( node.ChildNodes[0].InnerText ) )
                {
                    node.ChildNodes[0].InnerText = userDto.Id.ToString();
                    node.ChildNodes[1].InnerText = userDto.FirstName;
                    node.ChildNodes[2].InnerText = userDto.LastName;
                    node.ChildNodes[3].InnerText = userDto.Username;
                    node.ChildNodes[4].InnerText = userDto.Password;
                    break;
                }
            }

            doc.Save( database );
        }

        // DELETE api/<controller>/5
        [HttpDelete( "{id}" )]
        public void Delete( int id )
        {
            XmlDocument doc = new XmlDocument();
            doc.Load( database );

            XmlNode parent = doc.SelectSingleNode( "//Users" );


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
