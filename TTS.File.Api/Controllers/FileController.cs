using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Charts.Shared.Api.Controllers;
using Charts.Shared.Logic.File;
using Charts.Shared.Logic.Models.File;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TTS.File.Logic;

namespace TTS.File.Api.Controllers
{
    /// <summary>
    /// Контроллер для работы с файлами
    /// </summary>
    public class FileController : BaseController
    {
        private readonly IFileLogic _fileLogic;


        public FileController(IFileLogic fileLogic)
        {
            _fileLogic = fileLogic;
        }

        [HttpPost("UploadFiles")]
        //[UserRoleAttributeExtension(RoleEnum.Client)]
        public async Task<IActionResult> UploadFiles()
        {
            try
            {
                var files = HttpContext.Request.Form.Files;
                var fileInformation = JsonConvert.DeserializeObject<IEnumerable<FileUploadEntry>>(
                    HttpContext.Request.Form["fileInformation"][0]);

                foreach (var file in files)
                {
                    if (file.Length == 0) continue;
                    using var inputStream = new MemoryStream();
                    await file.CopyToAsync(inputStream);
                    await _fileLogic.UploadFile(file.FileName, inputStream.ToArray());
                }

                var res = await _fileLogic.UpdateFileInformation(fileInformation.ToList());
                return Ok(res);
            }
            catch (Exception e)
            {
                return ExceptionResult(e);
            }
        }

        [HttpGet("List" + "/{applicationId}")]
        public async Task<IActionResult> DocumentList(Guid applicationId)
        {
            try
            {
                var res = await _fileLogic.DocumentList(applicationId);
                return Ok(res);
            }
            catch (Exception e)
            {
                return ExceptionResult(e);
            }

        }

        //[HttpPost("UpdateDocInformation")]
        //public async Task<IActionResult> UpdateDocInformation(DocInformationsInDto model)
        //{
        //    try
        //    {
        //        return Ok(await _logic.UpdateDocInformation(model));
        //    }
        //    catch (Exception e)
        //    {
        //        return ExceptionResult(e);
        //    }
        //}

        [HttpGet("DownloadFile/{id}")]
        public async Task<IActionResult> DownloadFile(string id)
        {
            try
            {
                var file = await _fileLogic.GetFileById(Guid.Parse(id));
                if (file == null)
                    return NotFound();

                var buffer = await _fileLogic.ReadAsync(file.Path);
                if (buffer == null)
                    return NotFound();

                return new FileContentResult(buffer, file.ContentType)
                {
                    FileDownloadName = file.Name
                };
            }
            catch (Exception e)
            {
                return ExceptionResult(e);
            }

        }

        [HttpGet("FileToPdf/{id}")]
        public async Task<IActionResult> FileToPdf(string id)
        {
            try
            {
                var fileMemoryStream = new MemoryStream();
                var file = await _fileLogic.GetFileById(Guid.Parse(id));
                if (file == null)
                    return NotFound();

                var buffer = await _fileLogic.ReadAsync(file.Path);
                if (buffer == null)
                    return NotFound();

               var pdf = await _fileLogic.ConvertPdf2Pdf(fileMemoryStream, file.Path);
               if (!pdf)
                   return NotFound();

                fileMemoryStream.Position = 0;
                return new FileStreamResult(fileMemoryStream, "application/pdf");
            }
            catch (Exception e)
            {
                return ExceptionResult(e);
            }

        }



        //[HttpGet("DownloadListOfDocuments")]
        //public async Task<IActionResult> DownloadListOfDocuments()
        //{
        //    try
        //    {
        //        var buffer = await _logic.ReadAsync(Path.Combine(Environment.CurrentDirectory, @"Files\", "Перечень документов.pdf"));
        //        if (buffer == null)
        //            return NotFound();
        //        return new FileContentResult(buffer, "application/pdf")
        //        {
        //            FileDownloadName = "Перечень документов.pdf"
        //        };
        //    }
        //    catch (Exception e)
        //    {
        //        return ExceptionResult(e);
        //    }
        //}

        //[HttpPost("List")]
        //public async Task<IActionResult> DocumentList(AppFilesInDto appFiles)
        //{
        //    try
        //    {
        //        var res = await _logic.DocumentList(appFiles);
        //        return Ok(res);
        //    }
        //    catch (Exception e)
        //    {
        //        return ExceptionResult(e);
        //    }

        //}



        [HttpGet("DeleteFiles/{id}")]
        public async Task<IActionResult> DeleteFiles(Guid id)
        {
            try
            {
                await _fileLogic.DeleteFiles(id);
                return NoContent();
            }
            catch (Exception e)
            {
                return ExceptionResult(e);
            }
        }
    }
}
