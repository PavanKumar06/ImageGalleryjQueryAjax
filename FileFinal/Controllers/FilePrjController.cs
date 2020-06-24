using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using FileFinal.Models;

namespace FileFinal.Controllers
{
    public class FilePrjController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> UploadFile()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            var root = HttpContext.Current.Server.MapPath("~/App_Data");//This line is for inserting the file into the Data folder.
            var provider = new MultipartFormDataStreamProvider(root);
            var dbObj = new ADPFileDb();
            try
            {
                await Request.Content.ReadAsMultipartAsync(provider);//Readas... will insert data into provider.
                foreach (var file in provider.FileData)//We need to loop over the data in provider.
                {
                    var name = file.Headers.ContentDisposition.FileName;//To get the name of the file.
                    name = name.Trim('"');//The name consists of double quotes that is why we are removing it.
                    var localFileName = file.LocalFileName;//Source.
                    string filePath = Path.Combine(root, name);//Destination.
                    File.Move(localFileName, filePath);
                    dbObj = new ADPFileDb()
                    {
                        UserName = HttpContext.Current.Request.Params["user"],
                        FileName = Path.GetFileName(filePath).Substring(0, Path.GetFileName(filePath).LastIndexOf('.')),
                        FileType = filePath.Substring(filePath.LastIndexOf('.') + 1),
                        FileSize = (int)new FileInfo(filePath).Length,
                        Date = HttpContext.Current.Request.Params["datime"],
                        Location = filePath
                    };
                    using (var en = new SampleEntities())
                    {
                        en.ADPFileDb.Add(dbObj);
                        en.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
            return Request.CreateResponse(HttpStatusCode.OK, dbObj);
        }
        public HttpResponseMessage Get()
        {
            try
            {
                using (SampleEntities en = new SampleEntities())
                {
                    var entity = en.ADPFileDb.ToList();
                    return Request.CreateResponse(HttpStatusCode.OK, entity);
                }
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }
        public HttpResponseMessage Get(string uname)
        {
            try
            {
                using (SampleEntities en = new SampleEntities())
                {
                    var entity = en.ADPFileDb.Where(i => i.UserName == uname).ToList();
                    if (entity != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, entity);
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "The specified ID does not exist!!!");
                    }
                }
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                using (SampleEntities en = new SampleEntities())
                {
                    var entity = en.ADPFileDb.FirstOrDefault(i => i.Id == id);
                    if (entity != null)
                    {
                        File.Delete(entity.Location);
                        en.ADPFileDb.Remove(entity);
                        en.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, entity);
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "The specified ID does not exist!!!");
                    }
                }
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }
        public HttpResponseMessage Put(string fname, ADPFileDb d)
        {
            try
            {
                using (SampleEntities en = new SampleEntities())
                {
                    var entity = en.ADPFileDb.FirstOrDefault(i => i.FileName == fname);
                    if (entity != null)
                    {
                        entity.UserName = d.UserName;
                        entity.Date = d.Date;
                        en.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.Created, entity);
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "The specified ID does not exist!!!");
                    }
                }

            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }
    }
}