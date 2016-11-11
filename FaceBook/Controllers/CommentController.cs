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
    public class CommentController : ApiController
    {   /// <summary>
        /// Declare variables
        /// </summary>
        private string imgFolder = "/Images/profileimages/";
        private string defaultAvatar = "user.png";
        /// <summary>
        /// Database Connection
        /// </summary>
        private WallPostDBEntities2 db = new WallPostDBEntities2();
        /// <summary>
        /// To fetch all the comments under each post
        /// </summary>
        /// <returns></returns>
        // GET api/Comment
        public IEnumerable<PostComment> GetPostComments()
        {
            var postcomments = db.PostComments.Include(p => p.Post);
            return postcomments.AsEnumerable();
        }
        /// <summary>
        /// To fetch a particular comment based on id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET api/Comment/5
        public PostComment GetPostComment(int id)
        {
            PostComment postcomment = db.PostComments.Find(id);
            if (postcomment == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return postcomment;
        }
        /// <summary>
        /// Update a comment based on id with the PostComment object
        /// </summary>
        /// <param name="id"></param>
        /// <param name="postcomment"></param>
        /// <returns></returns>
        // PUT api/Comment/5
        public HttpResponseMessage PutPostComment(int id, PostComment postcomment)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != postcomment.CommentId)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(postcomment).State = EntityState.Modified;
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
        /// Insert new postcomment
        /// </summary>
        /// <param name="postcomment"></param>
        /// <returns></returns>
        // POST api/Comment
        public HttpResponseMessage PostPostComment(PostComment postcomment)
        {
            postcomment.CommentedBy = WebSecurity.CurrentUserId;
            postcomment.CommentedDate = DateTime.UtcNow;
            ModelState.Remove("postcomment.CommentedBy");
            ModelState.Remove("postcomment.CommentedDate");
            if (ModelState.IsValid)
            {
                db.PostComments.Add(postcomment);
                db.SaveChanges();
                var usr = db.UserProfiles.FirstOrDefault(x => x.UserId == postcomment.CommentedBy);
                var ret = new
                {
                    CommentedBy = postcomment.CommentedBy,
                    CommentedByName = usr.UserName,
                    CommentedByAvatar = imgFolder + (String.IsNullOrEmpty(usr.AvatarExt) ? defaultAvatar : postcomment.CommentedBy + "." + postcomment.UserProfile.AvatarExt),
                    CommentedDate = postcomment.CommentedDate,
                    CommentId = postcomment.CommentId,
                    Message = postcomment.Message,
                    PostId = postcomment.PostId
                };

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, ret);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = postcomment.CommentId }));
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