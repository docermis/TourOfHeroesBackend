using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourOfHeroesBackend.Models;
using System.IO;
using System.Text;
using System.Xml.Serialization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TourOfHeroesBackend.Controllers
{
    [Route( "api/[controller]" )]
    public class UsersController : Controller
    {
        string database = "UserDatabase.xml";

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
        public string Get( int id )
        {
            return "value";
        }

        // POST api/<controller>/register
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

            XmlNode userRef = doc.SelectSingleNode( "//Users" ).ChildNodes[count - 1];

            XmlNode userNode = doc.CreateElement( "User" );


            XmlNode node = doc.CreateElement( "id" );
            node.InnerText = ( id ).ToString();
            userNode.AppendChild( node );
            user.Id = id;

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

        [HttpPost( "authenticate" )]
        public string Authenticate( [FromBody] string username, string password )
        {


            return username;
        }


        // POST api/<controller>
        //[AllowAnonymous]
        //[HttpPost]
        //public IActionResult Register( [FromBody]UserDto userDto )
        //{
        //    // map dto to entity
        //    var user = _mapper.Map<User>( userDto );

        //    try
        //    {
        //        // save 
        //        _userService.Create( user, userDto.Password );
        //        return Ok();
        //    }
        //    catch ( AppException ex )
        //    {
        //        // return error message if there was an exception
        //        return BadRequest( ex.Message );
        //    }

        //    public User Create( User user, string password )
        //    {
        //        // validation
        //        if ( string.IsNullOrWhiteSpace( password ) )
        //            throw new AppException( "Password is required" );

        //        if ( _context.Users.Any( x => x.Username == user.Username ) )
        //            throw new AppException( "Username " + user.Username + " is already taken" );

        //        byte[] passwordHash, passwordSalt;
        //        if ( password == null ) throw new ArgumentNullException( "password" );
        //        if ( string.IsNullOrWhiteSpace( password ) ) throw new ArgumentException( "Value cannot be empty or whitespace only string.", "password" );

        //        using ( var hmac = new System.Security.Cryptography.HMACSHA512() )
        //        {
        //            passwordSalt = hmac.Key;
        //            passwordHash = hmac.ComputeHash( System.Text.Encoding.UTF8.GetBytes( password ) );
        //        }

        //        user.PasswordHash = passwordHash;
        //        user.PasswordSalt = passwordSalt;

        //        _context.Users.Add( user );
        //        _context.SaveChanges();

        //        return user;
        //    }


        //}

        // PUT api/<controller>/5
        [HttpPut( "{id}" )]
        public void Put( int id, [FromBody]string value )
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete( "{id}" )]
        public void Delete( int id )
        {
        }
    }
}
