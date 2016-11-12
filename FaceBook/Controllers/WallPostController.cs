using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using FaceBook.Models;
using WebMatrix.WebData;
namespace FaceBook.Controllers
{
    public class WallPostController : ApiController
    {
        /// <summary>
        /// Declare variable needed
        /// </summary>
        private string imgFolder = "~/Images/profileimages/";
        private string defaultAvatar = "user.png";
        /// <summary>
        /// The connection to the DB
        /// </summary>
        private WallPostDBEntities2 db = new WallPostDBEntities2();

        /// <summary>
        /// Fetch the posts using this webapi
        /// </summary>
        /// <returns>All Posts</returns>

        // GET api/WallPost
        public dynamic GetPosts()
        {

            var ret = (from post in db.Posts.ToList()
                       orderby post.PostedDate descending
                       select new
                       {
                           Message = post.Message,
                           PostedBy = post.PostedBy,
                           PostedByName = post.UserProfile.UserName,
                           PostedByAvatar = imgFolder + (String.IsNullOrEmpty(post.UserProfile.AvatarExt) ? defaultAvatar : post.PostedBy + "." + post.UserProfile.AvatarExt),
                           PostedDate = post.PostedDate,
                           PostId = post.PostId,
                           PostComments = from comment in post.PostComments.ToList()
                                          orderby comment.CommentedDate
                                          select new
                                          {
                                              CommentedBy = comment.CommentedBy,
                                              CommentedByName = comment.UserProfile.UserName,
                                              CommentedByAvatar = imgFolder + (String.IsNullOrEmpty(comment.UserProfile.AvatarExt) ? defaultAvatar : comment.CommentedBy + "." + comment.UserProfile.AvatarExt),
                                              CommentedDate = comment.CommentedDate,
                                              CommentId = comment.CommentId,
                                              Message = comment.Message,
                                              PostId = comment.PostId

                                          }
                       }).AsEnumerable();
            return ret;
        }

        /// <summary>
        /// Fetch the post based on Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>post</returns>      

        // GET api/WallPost/5
        public Post GetPost(int id)
        {
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return post;
        }

        /// <summary>
        /// Update a post based on id, passing the updated post object
        /// </summary>
        /// <param name="id"></param>
        /// <param name="post"></param>
        /// <returns></returns>
        // PUT api/WallPost/5
        public HttpResponseMessage PutPost(int id, Post post)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != post.PostId)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(post).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }
        /// <summary>
        /// Insert a post
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        // POST api/WallPost
        public HttpResponseMessage PostPost(Post post)
        {
            post.PostedBy = WebSecurity.CurrentUserId;
            post.PostedDate = DateTime.UtcNow;
            ModelState.Remove("post.PostedBy");
            ModelState.Remove("post.PostedDate");

            if (ModelState.IsValid)
            {
                db.Posts.Add(post);
                db.SaveChanges();
                var usr = db.UserProfiles.FirstOrDefault(x => x.UserId == post.PostedBy);
                var ret = new
                {
                    Message = post.Message,
                    PostedBy = post.PostedBy,
                    PostedByName = usr.UserName,
                    PostedByAvatar = imgFolder + (String.IsNullOrEmpty(usr.AvatarExt) ? defaultAvatar : post.PostedBy + "." + post.UserProfile.AvatarExt),
                    PostedDate = post.PostedDate,
                    PostId = post.PostId
                };
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, ret);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = post.PostId }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }       

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}