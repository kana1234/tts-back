using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Charts.Shared.Data.Context;
using Charts.Shared.Logic.Models.File;
using Microsoft.EntityFrameworkCore;
namespace Charts.Shared.Logic.File
{
    public class FileLogic : IFileLogic
    {
        private readonly IBaseLogic _iBaseLogic;

        public FileLogic(IBaseLogic iBaseLogic)
        {
            _iBaseLogic = iBaseLogic;
        }


        public async Task<TTSFile> UploadFile(string fileName, byte[] fileBytes)
        {
            fileName = NormalizeName(fileName);
            GeneratePath(fileName, out string filePath);
            System.IO.File.WriteAllBytes(filePath, fileBytes);

            TTSFile file = new TTSFile()
            {
                Name = fileName,
                Size = fileBytes.Length,
                Path = filePath,
                ContentType = ContentTypeHelper.GetContentType(Path.GetExtension(fileName)),
            };

            await _iBaseLogic.Of<TTSFile>().Add(file);
            return file;
        }

        public async Task<object> DocumentList(Guid applicationId)
        {
            var query = _iBaseLogic.Of<TTSFile>().GetQueryable(x => x.ApplicationId.Equals(applicationId)).AsNoTracking();

           

            return await query
                .Select(x => new
                {
                    x.ContentType,
                    x.Id,
                    x.FileType,
                    x.IsActual,
                    x.Name,
                    x.Path,
                    x.Size,
                    x.ApplicationId
                })
                .ToListAsync();
        }

        public async Task<object> UpdateFileInformation(List<FileUploadEntry> information)
        {
            information.ForEach(a=>a.Identifier = NormalizeName(a.Identifier));
            var identifiers = information.Select(x => x.Identifier);

            var files = await _iBaseLogic.Of<TTSFile>().Base()
                .Where(x => identifiers.Contains(x.Name))
                .ToListAsync();
            var result = new List<FileInDto>();
            foreach (var i in information)
            {
                var file = files.FirstOrDefault(f => f.Name ==i.Identifier);
                if (file == null) continue;


                file.ApplicationId = i.AppicationId;
                file.FileType = i.FileType;
                await _iBaseLogic.Of<TTSFile>().Save();

                result.Add(new FileInDto()
                {
                    Id = file.Id,
                    AppId = file.ApplicationId,
                    Size = file.Size,
                    Type = file.ContentType,
                    Title = file.Name
                });
            }
            return result;
        }

        //public async Task<Guid> ZipFiles(List<AgroFile> files, string fileName, Guid fileTypeId, string appNumber, Guid? userId, Guid applicationId)
        //{
        //    if (files.Count == 0)
        //    {
        //        throw new ArgumentException("Список файлов не может быть пустым");
        //    }

        //    GeneratePath(fileName, out string filePath);
        //    byte[] fileBytes;
        //    using (var memoryStream = new MemoryStream())
        //    {
        //        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        //        {
        //            var i = 1;
        //            foreach (var file in files)
        //            {
        //                string name = _dictionaryLogic.DictionaryRepo<DicFileType>().GetQueryable(x => x.Code.Equals(file.DocInformation.DocumentCode)).FirstOrDefault().NameRu;
        //                archive.CreateEntryFromFile(file.Path, name + Path.GetExtension(file.Name));
        //                i++;
        //            }
        //        }

        //        using (var fileStream = new FileStream(filePath, FileMode.Create))
        //        {
        //            memoryStream.Seek(0, SeekOrigin.Begin);
        //            memoryStream.CopyTo(fileStream);
        //            fileBytes = memoryStream.ToArray();
        //        }
        //    }
        //    return await _agroFileRepo.Add(
        //        new AgroFile()
        //        {
        //            Name = fileName,
        //            FileTypeId = fileTypeId,
        //            Size = fileBytes.Length,
        //            Path = filePath,
        //            ContentType = ContentTypeHelper.GetContentType(Path.GetExtension(fileName)),
        //            CreatorUserId = userId,
        //            ApplicationId = applicationId
        //        });
        //}

        //public async Task<bool> UpdateDocInformation(DocInformationsInDto model)
        //{
        //    if (model.Items.Count == 0) return false;

        //    var applicationId = model.Items.First().ApplicationId;
        //    var docs = await _agroFileRepo.Base()
        //        .Include(x => x.DocInformation)
        //        .Where(x => x.ApplicationId == applicationId)
        //        .Select(x => x.DocInformation)
        //        .ToListAsync();

        //    docs.ForEach(doc =>
        //    {
        //        var m = model.Items.FirstOrDefault(x => x.Code == doc.DocumentCode);
        //        if (m == null) return;

        //        doc.IsOriginal = m.IsOriginal;
        //        doc.PageCount = m.PageCount;
        //        doc.PageInterval = m.PageInterval;
        //    });

        //    await _agroFileRepo.Save();
        //    return true;
        //}

        public async Task<TTSFile> GetFileById(Guid id)
        {
            TTSFile res = await _iBaseLogic.Of<TTSFile>().GetQueryable(x => x.Id.Equals(id)).FirstOrDefaultAsync();
            return res;
        }
        //public async Task<object> SetDocInformation(DocInformation docInfo, AgroFile file)
        //{
        //    await _docInformationRepo.Add(docInfo);
        //    await _docInformationRepo.Save();

        //    file.DocInformationId = docInfo.Id;
        //    await _agroFileRepo.Update(file);
        //    await _agroFileRepo.Save();

        //    return docInfo;
        //}

        public Task<byte[]> ReadAsync(string filePath)
        {
            byte[] buff = null;
            FileStream fs = new FileStream(filePath,
                                           FileMode.Open,
                                           FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            long numBytes = new FileInfo(filePath).Length;
            buff = br.ReadBytes((int)numBytes);
            fs.Close();
            return Task.FromResult(buff);
        }

        public async Task<bool> ConvertPdf2Pdf(MemoryStream memoryStream, string path)
        {
            try
            {

                var fileStream = System.IO.File.ReadAllBytes(path);
                MemoryStream ms = new MemoryStream(fileStream);
                ms.CopyTo(memoryStream);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private string NormalizeName(string fileName)
        {
            fileName = fileName.Replace(' ', '_');
            if (fileName.Length < 100)
            {
                return fileName;
            }

            var ext = Path.GetExtension(fileName);
            var name = Path.GetFileNameWithoutExtension(fileName);

            return $"{name.Substring(0, 15)}~{ext}";
        }
        private void GeneratePath(string fileName, out string filePath)
        {
            var today = DateTime.Today;
            var guid = Guid.NewGuid();

            var fullPath = string.Format("{0}{1:yyyyMM}\\{1:dd}\\{2:N}", @"C:\TTS-FILES\", today, guid);
            if (Directory.Exists(fullPath) == false)
            {
                Directory.CreateDirectory(fullPath);
            }
            filePath = Path.Combine(fullPath, fileName);
        }

        //public async Task<object> DocumentList(AppFilesInDto appFiles)
        //{
        //    var query = _agroFileRepo.Base().AsNoTracking();

        //    if (appFiles.Id != null)
        //        query = query.Where(x => x.Id == appFiles.Id);

        //    if (appFiles.AppId != null)
        //        query = query.Where(x => x.ApplicationId.Equals(appFiles.AppId));

        //    if (appFiles.Page != null)
        //        query = query.Where(x => x.DocInformation.Page.Equals(appFiles.Page));

        //    if (appFiles.Codes.Count > 0)
        //        query = query.Where(x => appFiles.Codes.Contains(x.FileType.Code));

        //    if (appFiles.BasePledgeId != null)
        //        query = query.Where(x => x.BasePledgeId.Equals(appFiles.BasePledgeId));

        //    return await query
        //        .Select(x => new AgroFileInDto()
        //        {
        //            Id = x.Id,
        //            AppId = x.ApplicationId,
        //            BasePledgeId = x.BasePledgeId,
        //            Size = x.Size,
        //            Type = x.ContentType,
        //            Code = x.DocInformation.DocumentCode,
        //            Page = x.DocInformation.Page,
        //            Date = x.DocInformation.Date,
        //            Number = x.DocInformation.Number,
        //            Title = x.Name,

        //            IsOriginal = x.DocInformation.IsOriginal,
        //            PageCount = x.DocInformation.PageCount,
        //            PageInterval = x.DocInformation.PageInterval
        //        })
        //        .ToListAsync();
        //}

        

        public async Task DeleteFiles(IEnumerable<Guid> fileIds)
        {
            var files = new List<TTSFile>();

            foreach (Guid id in fileIds)
            {
                var file = await GetFileById(id);
                files.Add(file);

               // var docInfo = await GetDocInformationById(file.DocInformationId ?? Guid.Empty);

                //if (docInfo != null)
                //    await _docInformationRepo.Remove((DocInformation)docInfo);

                if (System.IO.File.Exists(file.Path))
                    System.IO.File.Delete(file.Path);
            }

            await _iBaseLogic.Of<TTSFile>().RemoveRange(files);
        }

        public async Task DeleteFiles(Guid fileId)
        {

            var file = await GetFileById(fileId);
            if (System.IO.File.Exists(file.Path))
                System.IO.File.Delete(file.Path);
            await _iBaseLogic.Of<TTSFile>().Remove(file);
        }
        //public async Task<object> GetDocInformationById(Guid id)
        //{
        //    var res = await _docInformationRepo.GetQueryable(x => x.Id.Equals(id)).FirstOrDefaultAsync();
        //    return res;
        //}
    }
    public class ContentTypeDto
    {
        public ContentTypeDto(string extension, string contentType)
        {
            Extension = extension;
            ContentType = contentType;
        }

        public string Extension { get; set; }
        public string ContentType { get; set; }
    }
    public class ContentTypeHelper
    {
        public static string GetContentType(string extension)
        {
            var fileTypes = GetAllFileTypes();
            return fileTypes.FirstOrDefault(x => x.Extension == extension)?.ContentType;
        }

        static ContentTypeDto[] GetAllFileTypes()
        {
            return new[]
            {
        new ContentTypeDto(".323", "text/h323"),
        new ContentTypeDto(".3g2", "video/3gpp2"),
        new ContentTypeDto(".3gp", "video/3gpp"),
        new ContentTypeDto(".3gp2", "video/3gpp2"),
        new ContentTypeDto(".3gpp", "video/3gpp"),
        new ContentTypeDto(".7z", "application/x-7z-compressed"),
        new ContentTypeDto(".aa", "audio/audible"),
        new ContentTypeDto(".AAC", "audio/aac"),
        new ContentTypeDto(".aaf", "application/octet-stream"),
        new ContentTypeDto(".aax", "audio/vnd.audible.aax"),
        new ContentTypeDto(".ac3", "audio/ac3"),
        new ContentTypeDto(".aca", "application/octet-stream"),
        new ContentTypeDto(".accda", "application/msaccess.addin"),
        new ContentTypeDto(".accdb", "application/msaccess"),
        new ContentTypeDto(".accdc", "application/msaccess.cab"),
        new ContentTypeDto(".accde", "application/msaccess"),
        new ContentTypeDto(".accdr", "application/msaccess.runtime"),
        new ContentTypeDto(".accdt", "application/msaccess"),
        new ContentTypeDto(".accdw", "application/msaccess.webapplication"),
        new ContentTypeDto(".accft", "application/msaccess.ftemplate"),
        new ContentTypeDto(".acx", "application/internet-property-stream"),
        new ContentTypeDto(".AddIn", "text/xml"),
        new ContentTypeDto(".ade", "application/msaccess"),
        new ContentTypeDto(".adobebridge", "application/x-bridge-url"),
        new ContentTypeDto(".adp", "application/msaccess"),
        new ContentTypeDto(".ADT", "audio/vnd.dlna.adts"),
        new ContentTypeDto(".ADTS", "audio/aac"),
        new ContentTypeDto(".afm", "application/octet-stream"),
        new ContentTypeDto(".ai", "application/postscript"),
        new ContentTypeDto(".aif", "audio/x-aiff"),
        new ContentTypeDto(".aifc", "audio/aiff"),
        new ContentTypeDto(".aiff", "audio/aiff"),
        new ContentTypeDto(".air", "application/vnd.adobe.air-application-installer-package+zip"),
        new ContentTypeDto(".amc", "application/x-mpeg"),
        new ContentTypeDto(".application", "application/x-ms-application"),
        new ContentTypeDto(".art", "image/x-jg"),
        new ContentTypeDto(".asa", "application/xml"),
        new ContentTypeDto(".asax", "application/xml"),
        new ContentTypeDto(".ascx", "application/xml"),
        new ContentTypeDto(".asd", "application/octet-stream"),
        new ContentTypeDto(".asf", "video/x-ms-asf"),
        new ContentTypeDto(".ashx", "application/xml"),
        new ContentTypeDto(".asi", "application/octet-stream"),
        new ContentTypeDto(".asm", "text/plain"),
        new ContentTypeDto(".asmx", "application/xml"),
        new ContentTypeDto(".aspx", "application/xml"),
        new ContentTypeDto(".asr", "video/x-ms-asf"),
        new ContentTypeDto(".asx", "video/x-ms-asf"),
        new ContentTypeDto(".atom", "application/atom+xml"),
        new ContentTypeDto(".au", "audio/basic"),
        new ContentTypeDto(".avi", "video/x-msvideo"),
        new ContentTypeDto(".axs", "application/olescript"),
        new ContentTypeDto(".bas", "text/plain"),
        new ContentTypeDto(".bcpio", "application/x-bcpio"),
        new ContentTypeDto(".bin", "application/octet-stream"),
        new ContentTypeDto(".bmp", "image/bmp"),
        new ContentTypeDto(".c", "text/plain"),
        new ContentTypeDto(".cab", "application/octet-stream"),
        new ContentTypeDto(".caf", "audio/x-caf"),
        new ContentTypeDto(".calx", "application/vnd.ms-office.calx"),
        new ContentTypeDto(".cat", "application/vnd.ms-pki.seccat"),
        new ContentTypeDto(".cc", "text/plain"),
        new ContentTypeDto(".cd", "text/plain"),
        new ContentTypeDto(".cdda", "audio/aiff"),
        new ContentTypeDto(".cdf", "application/x-cdf"),
        new ContentTypeDto(".cer", "application/x-x509-ca-cert"),
        new ContentTypeDto(".chm", "application/octet-stream"),
        new ContentTypeDto(".class", "application/x-java-applet"),
        new ContentTypeDto(".clp", "application/x-msclip"),
        new ContentTypeDto(".cmx", "image/x-cmx"),
        new ContentTypeDto(".cnf", "text/plain"),
        new ContentTypeDto(".cod", "image/cis-cod"),
        new ContentTypeDto(".config", "application/xml"),
        new ContentTypeDto(".contact", "text/x-ms-contact"),
        new ContentTypeDto(".coverage", "application/xml"),
        new ContentTypeDto(".cpio", "application/x-cpio"),
        new ContentTypeDto(".cpp", "text/plain"),
        new ContentTypeDto(".crd", "application/x-mscardfile"),
        new ContentTypeDto(".crl", "application/pkix-crl"),
        new ContentTypeDto(".crt", "application/x-x509-ca-cert"),
        new ContentTypeDto(".cs", "text/plain"),
        new ContentTypeDto(".csdproj", "text/plain"),
        new ContentTypeDto(".csh", "application/x-csh"),
        new ContentTypeDto(".csproj", "text/plain"),
        new ContentTypeDto(".css", "text/css"),
        new ContentTypeDto(".csv", "text/csv"),
        new ContentTypeDto(".cur", "application/octet-stream"),
        new ContentTypeDto(".cxx", "text/plain"),
        new ContentTypeDto(".dat", "application/octet-stream"),
        new ContentTypeDto(".datasource", "application/xml"),
        new ContentTypeDto(".dbproj", "text/plain"),
        new ContentTypeDto(".dcr", "application/x-director"),
        new ContentTypeDto(".def", "text/plain"),
        new ContentTypeDto(".deploy", "application/octet-stream"),
        new ContentTypeDto(".der", "application/x-x509-ca-cert"),
        new ContentTypeDto(".dgml", "application/xml"),
        new ContentTypeDto(".dib", "image/bmp"),
        new ContentTypeDto(".dif", "video/x-dv"),
        new ContentTypeDto(".dir", "application/x-director"),
        new ContentTypeDto(".disco", "text/xml"),
        new ContentTypeDto(".dll", "application/x-msdownload"),
        new ContentTypeDto(".dll.config", "text/xml"),
        new ContentTypeDto(".dlm", "text/dlm"),
        new ContentTypeDto(".doc", "application/msword"),
        new ContentTypeDto(".docm", "application/vnd.ms-word.document.macroEnabled.12"),
        new ContentTypeDto(".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"),
        new ContentTypeDto(".dot", "application/msword"),
        new ContentTypeDto(".dotm", "application/vnd.ms-word.template.macroEnabled.12"),
        new ContentTypeDto(".dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template"),
        new ContentTypeDto(".dsp", "application/octet-stream"),
        new ContentTypeDto(".dsw", "text/plain"),
        new ContentTypeDto(".dtd", "text/xml"),
        new ContentTypeDto(".dtsConfig", "text/xml"),
        new ContentTypeDto(".dv", "video/x-dv"),
        new ContentTypeDto(".dvi", "application/x-dvi"),
        new ContentTypeDto(".dwf", "drawing/x-dwf"),
        new ContentTypeDto(".dwp", "application/octet-stream"),
        new ContentTypeDto(".dxr", "application/x-director"),
        new ContentTypeDto(".eml", "message/rfc822"),
        new ContentTypeDto(".emz", "application/octet-stream"),
        new ContentTypeDto(".eot", "application/octet-stream"),
        new ContentTypeDto(".eps", "application/postscript"),
        new ContentTypeDto(".etl", "application/etl"),
        new ContentTypeDto(".etx", "text/x-setext"),
        new ContentTypeDto(".evy", "application/envoy"),
        new ContentTypeDto(".exe", "application/octet-stream"),
        new ContentTypeDto(".exe.config", "text/xml"),
        new ContentTypeDto(".fdf", "application/vnd.fdf"),
        new ContentTypeDto(".fif", "application/fractals"),
        new ContentTypeDto(".filters", "Application/xml"),
        new ContentTypeDto(".fla", "application/octet-stream"),
        new ContentTypeDto(".flr", "x-world/x-vrml"),
        new ContentTypeDto(".flv", "video/x-flv"),
        new ContentTypeDto(".fsscript", "application/fsharp-script"),
        new ContentTypeDto(".fsx", "application/fsharp-script"),
        new ContentTypeDto(".generictest", "application/xml"),
        new ContentTypeDto(".gif", "image/gif"),
        new ContentTypeDto(".group", "text/x-ms-group"),
        new ContentTypeDto(".gsm", "audio/x-gsm"),
        new ContentTypeDto(".gtar", "application/x-gtar"),
        new ContentTypeDto(".gz", "application/x-gzip"),
        new ContentTypeDto(".h", "text/plain"),
        new ContentTypeDto(".hdf", "application/x-hdf"),
        new ContentTypeDto(".hdml", "text/x-hdml"),
        new ContentTypeDto(".hhc", "application/x-oleobject"),
        new ContentTypeDto(".hhk", "application/octet-stream"),
        new ContentTypeDto(".hhp", "application/octet-stream"),
        new ContentTypeDto(".hlp", "application/winhlp"),
        new ContentTypeDto(".hpp", "text/plain"),
        new ContentTypeDto(".hqx", "application/mac-binhex40"),
        new ContentTypeDto(".hta", "application/hta"),
        new ContentTypeDto(".htc", "text/x-component"),
        new ContentTypeDto(".htm", "text/html"),
        new ContentTypeDto(".html", "text/html"),
        new ContentTypeDto(".htt", "text/webviewhtml"),
        new ContentTypeDto(".hxa", "application/xml"),
        new ContentTypeDto(".hxc", "application/xml"),
        new ContentTypeDto(".hxd", "application/octet-stream"),
        new ContentTypeDto(".hxe", "application/xml"),
        new ContentTypeDto(".hxf", "application/xml"),
        new ContentTypeDto(".hxh", "application/octet-stream"),
        new ContentTypeDto(".hxi", "application/octet-stream"),
        new ContentTypeDto(".hxk", "application/xml"),
        new ContentTypeDto(".hxq", "application/octet-stream"),
        new ContentTypeDto(".hxr", "application/octet-stream"),
        new ContentTypeDto(".hxs", "application/octet-stream"),
        new ContentTypeDto(".hxt", "text/html"),
        new ContentTypeDto(".hxv", "application/xml"),
        new ContentTypeDto(".hxw", "application/octet-stream"),
        new ContentTypeDto(".hxx", "text/plain"),
        new ContentTypeDto(".i", "text/plain"),
        new ContentTypeDto(".ico", "image/x-icon"),
        new ContentTypeDto(".ics", "application/octet-stream"),
        new ContentTypeDto(".idl", "text/plain"),
        new ContentTypeDto(".ief", "image/ief"),
        new ContentTypeDto(".iii", "application/x-iphone"),
        new ContentTypeDto(".inc", "text/plain"),
        new ContentTypeDto(".inf", "application/octet-stream"),
        new ContentTypeDto(".inl", "text/plain"),
        new ContentTypeDto(".ins", "application/x-internet-signup"),
        new ContentTypeDto(".ipa", "application/x-itunes-ipa"),
        new ContentTypeDto(".ipg", "application/x-itunes-ipg"),
        new ContentTypeDto(".ipproj", "text/plain"),
        new ContentTypeDto(".ipsw", "application/x-itunes-ipsw"),
        new ContentTypeDto(".iqy", "text/x-ms-iqy"),
        new ContentTypeDto(".isp", "application/x-internet-signup"),
        new ContentTypeDto(".ite", "application/x-itunes-ite"),
        new ContentTypeDto(".itlp", "application/x-itunes-itlp"),
        new ContentTypeDto(".itms", "application/x-itunes-itms"),
        new ContentTypeDto(".itpc", "application/x-itunes-itpc"),
        new ContentTypeDto(".IVF", "video/x-ivf"),
        new ContentTypeDto(".jar", "application/java-archive"),
        new ContentTypeDto(".java", "application/octet-stream"),
        new ContentTypeDto(".jck", "application/liquidmotion"),
        new ContentTypeDto(".jcz", "application/liquidmotion"),
        new ContentTypeDto(".jfif", "image/pjpeg"),
        new ContentTypeDto(".jnlp", "application/x-java-jnlp-file"),
        new ContentTypeDto(".jpb", "application/octet-stream"),
        new ContentTypeDto(".jpe", "image/jpeg"),
        new ContentTypeDto(".jpeg", "image/jpeg"),
        new ContentTypeDto(".jpg", "image/jpeg"),
        new ContentTypeDto(".js", "application/x-javascript"),
        new ContentTypeDto(".json", "application/json"),
        new ContentTypeDto(".jsx", "text/jscript"),
        new ContentTypeDto(".jsxbin", "text/plain"),
        new ContentTypeDto(".latex", "application/x-latex"),
        new ContentTypeDto(".library-ms", "application/windows-library+xml"),
        new ContentTypeDto(".lit", "application/x-ms-reader"),
        new ContentTypeDto(".loadtest", "application/xml"),
        new ContentTypeDto(".lpk", "application/octet-stream"),
        new ContentTypeDto(".lsf", "video/x-la-asf"),
        new ContentTypeDto(".lst", "text/plain"),
        new ContentTypeDto(".lsx", "video/x-la-asf"),
        new ContentTypeDto(".lzh", "application/octet-stream"),
        new ContentTypeDto(".m13", "application/x-msmediaview"),
        new ContentTypeDto(".m14", "application/x-msmediaview"),
        new ContentTypeDto(".m1v", "video/mpeg"),
        new ContentTypeDto(".m2t", "video/vnd.dlna.mpeg-tts"),
        new ContentTypeDto(".m2ts", "video/vnd.dlna.mpeg-tts"),
        new ContentTypeDto(".m2v", "video/mpeg"),
        new ContentTypeDto(".m3u", "audio/x-mpegurl"),
        new ContentTypeDto(".m3u8", "audio/x-mpegurl"),
        new ContentTypeDto(".m4a", "audio/m4a"),
        new ContentTypeDto(".m4b", "audio/m4b"),
        new ContentTypeDto(".m4p", "audio/m4p"),
        new ContentTypeDto(".m4r", "audio/x-m4r"),
        new ContentTypeDto(".m4v", "video/x-m4v"),
        new ContentTypeDto(".mac", "image/x-macpaint"),
        new ContentTypeDto(".mak", "text/plain"),
        new ContentTypeDto(".man", "application/x-troff-man"),
        new ContentTypeDto(".manifest", "application/x-ms-manifest"),
        new ContentTypeDto(".map", "text/plain"),
        new ContentTypeDto(".master", "application/xml"),
        new ContentTypeDto(".mda", "application/msaccess"),
        new ContentTypeDto(".mdb", "application/x-msaccess"),
        new ContentTypeDto(".mde", "application/msaccess"),
        new ContentTypeDto(".mdp", "application/octet-stream"),
        new ContentTypeDto(".me", "application/x-troff-me"),
        new ContentTypeDto(".mfp", "application/x-shockwave-flash"),
        new ContentTypeDto(".mht", "message/rfc822"),
        new ContentTypeDto(".mhtml", "message/rfc822"),
        new ContentTypeDto(".mid", "audio/mid"),
        new ContentTypeDto(".midi", "audio/mid"),
        new ContentTypeDto(".mix", "application/octet-stream"),
        new ContentTypeDto(".mk", "text/plain"),
        new ContentTypeDto(".mmf", "application/x-smaf"),
        new ContentTypeDto(".mno", "text/xml"),
        new ContentTypeDto(".mny", "application/x-msmoney"),
        new ContentTypeDto(".mod", "video/mpeg"),
        new ContentTypeDto(".mov", "video/quicktime"),
        new ContentTypeDto(".movie", "video/x-sgi-movie"),
        new ContentTypeDto(".mp2", "video/mpeg"),
        new ContentTypeDto(".mp2v", "video/mpeg"),
        new ContentTypeDto(".mp3", "audio/mpeg"),
        new ContentTypeDto(".mp4", "video/mp4"),
        new ContentTypeDto(".mp4v", "video/mp4"),
        new ContentTypeDto(".mpa", "video/mpeg"),
        new ContentTypeDto(".mpe", "video/mpeg"),
        new ContentTypeDto(".mpeg", "video/mpeg"),
        new ContentTypeDto(".mpf", "application/vnd.ms-mediapackage"),
        new ContentTypeDto(".mpg", "video/mpeg"),
        new ContentTypeDto(".mpp", "application/vnd.ms-project"),
        new ContentTypeDto(".mpv2", "video/mpeg"),
        new ContentTypeDto(".mqv", "video/quicktime"),
        new ContentTypeDto(".ms", "application/x-troff-ms"),
        new ContentTypeDto(".msi", "application/octet-stream"),
        new ContentTypeDto(".mso", "application/octet-stream"),
        new ContentTypeDto(".mts", "video/vnd.dlna.mpeg-tts"),
        new ContentTypeDto(".mtx", "application/xml"),
        new ContentTypeDto(".mvb", "application/x-msmediaview"),
        new ContentTypeDto(".mvc", "application/x-miva-compiled"),
        new ContentTypeDto(".mxp", "application/x-mmxp"),
        new ContentTypeDto(".nc", "application/x-netcdf"),
        new ContentTypeDto(".nsc", "video/x-ms-asf"),
        new ContentTypeDto(".nws", "message/rfc822"),
        new ContentTypeDto(".ocx", "application/octet-stream"),
        new ContentTypeDto(".oda", "application/oda"),
        new ContentTypeDto(".odc", "text/x-ms-odc"),
        new ContentTypeDto(".odh", "text/plain"),
        new ContentTypeDto(".odl", "text/plain"),
        new ContentTypeDto(".odp", "application/vnd.oasis.opendocument.presentation"),
        new ContentTypeDto(".ods", "application/oleobject"),
        new ContentTypeDto(".odt", "application/vnd.oasis.opendocument.text"),
        new ContentTypeDto(".one", "application/onenote"),
        new ContentTypeDto(".onea", "application/onenote"),
        new ContentTypeDto(".onepkg", "application/onenote"),
        new ContentTypeDto(".onetmp", "application/onenote"),
        new ContentTypeDto(".onetoc", "application/onenote"),
        new ContentTypeDto(".onetoc2", "application/onenote"),
        new ContentTypeDto(".orderedtest", "application/xml"),
        new ContentTypeDto(".osdx", "application/opensearchdescription+xml"),
        new ContentTypeDto(".p10", "application/pkcs10"),
        new ContentTypeDto(".p12", "application/x-pkcs12"),
        new ContentTypeDto(".p7b", "application/x-pkcs7-certificates"),
        new ContentTypeDto(".p7c", "application/pkcs7-mime"),
        new ContentTypeDto(".p7m", "application/pkcs7-mime"),
        new ContentTypeDto(".p7r", "application/x-pkcs7-certreqresp"),
        new ContentTypeDto(".p7s", "application/pkcs7-signature"),
        new ContentTypeDto(".pbm", "image/x-portable-bitmap"),
        new ContentTypeDto(".pcast", "application/x-podcast"),
        new ContentTypeDto(".pct", "image/pict"),
        new ContentTypeDto(".pcx", "application/octet-stream"),
        new ContentTypeDto(".pcz", "application/octet-stream"),
        new ContentTypeDto(".pdf", "application/pdf"),
        new ContentTypeDto(".pfb", "application/octet-stream"),
        new ContentTypeDto(".pfm", "application/octet-stream"),
        new ContentTypeDto(".pfx", "application/x-pkcs12"),
        new ContentTypeDto(".pgm", "image/x-portable-graymap"),
        new ContentTypeDto(".pic", "image/pict"),
        new ContentTypeDto(".pict", "image/pict"),
        new ContentTypeDto(".pkgdef", "text/plain"),
        new ContentTypeDto(".pkgundef", "text/plain"),
        new ContentTypeDto(".pko", "application/vnd.ms-pki.pko"),
        new ContentTypeDto(".pls", "audio/scpls"),
        new ContentTypeDto(".pma", "application/x-perfmon"),
        new ContentTypeDto(".pmc", "application/x-perfmon"),
        new ContentTypeDto(".pml", "application/x-perfmon"),
        new ContentTypeDto(".pmr", "application/x-perfmon"),
        new ContentTypeDto(".pmw", "application/x-perfmon"),
        new ContentTypeDto(".png", "image/png"),
        new ContentTypeDto(".pnm", "image/x-portable-anymap"),
        new ContentTypeDto(".pnt", "image/x-macpaint"),
        new ContentTypeDto(".pntg", "image/x-macpaint"),
        new ContentTypeDto(".pnz", "image/png"),
        new ContentTypeDto(".pot", "application/vnd.ms-powerpoint"),
        new ContentTypeDto(".potm", "application/vnd.ms-powerpoint.template.macroEnabled.12"),
        new ContentTypeDto(".potx", "application/vnd.openxmlformats-officedocument.presentationml.template"),
        new ContentTypeDto(".ppa", "application/vnd.ms-powerpoint"),
        new ContentTypeDto(".ppam", "application/vnd.ms-powerpoint.addin.macroEnabled.12"),
        new ContentTypeDto(".ppm", "image/x-portable-pixmap"),
        new ContentTypeDto(".pps", "application/vnd.ms-powerpoint"),
        new ContentTypeDto(".ppsm", "application/vnd.ms-powerpoint.slideshow.macroEnabled.12"),
        new ContentTypeDto(".ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow"),
        new ContentTypeDto(".ppt", "application/vnd.ms-powerpoint"),
        new ContentTypeDto(".pptm", "application/vnd.ms-powerpoint.presentation.macroEnabled.12"),
        new ContentTypeDto(".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"),
        new ContentTypeDto(".prf", "application/pics-rules"),
        new ContentTypeDto(".prm", "application/octet-stream"),
        new ContentTypeDto(".prx", "application/octet-stream"),
        new ContentTypeDto(".ps", "application/postscript"),
        new ContentTypeDto(".psc1", "application/PowerShell"),
        new ContentTypeDto(".psd", "application/octet-stream"),
        new ContentTypeDto(".psess", "application/xml"),
        new ContentTypeDto(".psm", "application/octet-stream"),
        new ContentTypeDto(".psp", "application/octet-stream"),
        new ContentTypeDto(".pub", "application/x-mspublisher"),
        new ContentTypeDto(".pwz", "application/vnd.ms-powerpoint"),
        new ContentTypeDto(".qht", "text/x-html-insertion"),
        new ContentTypeDto(".qhtm", "text/x-html-insertion"),
        new ContentTypeDto(".qt", "video/quicktime"),
        new ContentTypeDto(".qti", "image/x-quicktime"),
        new ContentTypeDto(".qtif", "image/x-quicktime"),
        new ContentTypeDto(".qtl", "application/x-quicktimeplayer"),
        new ContentTypeDto(".qxd", "application/octet-stream"),
        new ContentTypeDto(".ra", "audio/x-pn-realaudio"),
        new ContentTypeDto(".ram", "audio/x-pn-realaudio"),
        new ContentTypeDto(".rar", "application/octet-stream"),
        new ContentTypeDto(".ras", "image/x-cmu-raster"),
        new ContentTypeDto(".rat", "application/rat-file"),
        new ContentTypeDto(".rc", "text/plain"),
        new ContentTypeDto(".rc2", "text/plain"),
        new ContentTypeDto(".rct", "text/plain"),
        new ContentTypeDto(".rdlc", "application/xml"),
        new ContentTypeDto(".resx", "application/xml"),
        new ContentTypeDto(".rf", "image/vnd.rn-realflash"),
        new ContentTypeDto(".rgb", "image/x-rgb"),
        new ContentTypeDto(".rgs", "text/plain"),
        new ContentTypeDto(".rm", "application/vnd.rn-realmedia"),
        new ContentTypeDto(".rmi", "audio/mid"),
        new ContentTypeDto(".rmp", "application/vnd.rn-rn_music_package"),
        new ContentTypeDto(".roff", "application/x-troff"),
        new ContentTypeDto(".rpm", "audio/x-pn-realaudio-plugin"),
        new ContentTypeDto(".rqy", "text/x-ms-rqy"),
        new ContentTypeDto(".rtf", "application/rtf"),
        new ContentTypeDto(".rtx", "text/richtext"),
        new ContentTypeDto(".ruleset", "application/xml"),
        new ContentTypeDto(".s", "text/plain"),
        new ContentTypeDto(".safariextz", "application/x-safari-safariextz"),
        new ContentTypeDto(".scd", "application/x-msschedule"),
        new ContentTypeDto(".sct", "text/scriptlet"),
        new ContentTypeDto(".sd2", "audio/x-sd2"),
        new ContentTypeDto(".sdp", "application/sdp"),
        new ContentTypeDto(".sea", "application/octet-stream"),
        new ContentTypeDto(".searchConnector-ms", "application/windows-search-connector+xml"),
        new ContentTypeDto(".setpay", "application/set-payment-initiation"),
        new ContentTypeDto(".setreg", "application/set-registration-initiation"),
        new ContentTypeDto(".settings", "application/xml"),
        new ContentTypeDto(".sgimb", "application/x-sgimb"),
        new ContentTypeDto(".sgml", "text/sgml"),
        new ContentTypeDto(".sh", "application/x-sh"),
        new ContentTypeDto(".shar", "application/x-shar"),
        new ContentTypeDto(".shtml", "text/html"),
        new ContentTypeDto(".sit", "application/x-stuffit"),
        new ContentTypeDto(".sitemap", "application/xml"),
        new ContentTypeDto(".skin", "application/xml"),
        new ContentTypeDto(".sldm", "application/vnd.ms-powerpoint.slide.macroEnabled.12"),
        new ContentTypeDto(".sldx", "application/vnd.openxmlformats-officedocument.presentationml.slide"),
        new ContentTypeDto(".slk", "application/vnd.ms-excel"),
        new ContentTypeDto(".sln", "text/plain"),
        new ContentTypeDto(".slupkg-ms", "application/x-ms-license"),
        new ContentTypeDto(".smd", "audio/x-smd"),
        new ContentTypeDto(".smi", "application/octet-stream"),
        new ContentTypeDto(".smx", "audio/x-smd"),
        new ContentTypeDto(".smz", "audio/x-smd"),
        new ContentTypeDto(".snd", "audio/basic"),
        new ContentTypeDto(".snippet", "application/xml"),
        new ContentTypeDto(".snp", "application/octet-stream"),
        new ContentTypeDto(".sol", "text/plain"),
        new ContentTypeDto(".sor", "text/plain"),
        new ContentTypeDto(".spc", "application/x-pkcs7-certificates"),
        new ContentTypeDto(".spl", "application/futuresplash"),
        new ContentTypeDto(".src", "application/x-wais-source"),
        new ContentTypeDto(".srf", "text/plain"),
        new ContentTypeDto(".SSISDeploymentManifest", "text/xml"),
        new ContentTypeDto(".ssm", "application/streamingmedia"),
        new ContentTypeDto(".sst", "application/vnd.ms-pki.certstore"),
        new ContentTypeDto(".stl", "application/vnd.ms-pki.stl"),
        new ContentTypeDto(".sv4cpio", "application/x-sv4cpio"),
        new ContentTypeDto(".sv4crc", "application/x-sv4crc"),
        new ContentTypeDto(".svc", "application/xml"),
        new ContentTypeDto(".swf", "application/x-shockwave-flash"),
        new ContentTypeDto(".t", "application/x-troff"),
        new ContentTypeDto(".tar", "application/x-tar"),
        new ContentTypeDto(".tcl", "application/x-tcl"),
        new ContentTypeDto(".testrunconfig", "application/xml"),
        new ContentTypeDto(".testsettings", "application/xml"),
        new ContentTypeDto(".tex", "application/x-tex"),
        new ContentTypeDto(".texi", "application/x-texinfo"),
        new ContentTypeDto(".texinfo", "application/x-texinfo"),
        new ContentTypeDto(".tgz", "application/x-compressed"),
        new ContentTypeDto(".thmx", "application/vnd.ms-officetheme"),
        new ContentTypeDto(".thn", "application/octet-stream"),
        new ContentTypeDto(".tif", "image/tiff"),
        new ContentTypeDto(".tiff", "image/tiff"),
        new ContentTypeDto(".tlh", "text/plain"),
        new ContentTypeDto(".tli", "text/plain"),
        new ContentTypeDto(".toc", "application/octet-stream"),
        new ContentTypeDto(".tr", "application/x-troff"),
        new ContentTypeDto(".trm", "application/x-msterminal"),
        new ContentTypeDto(".trx", "application/xml"),
        new ContentTypeDto(".ts", "video/vnd.dlna.mpeg-tts"),
        new ContentTypeDto(".tsv", "text/tab-separated-values"),
        new ContentTypeDto(".ttf", "application/octet-stream"),
        new ContentTypeDto(".tts", "video/vnd.dlna.mpeg-tts"),
        new ContentTypeDto(".txt", "text/plain"),
        new ContentTypeDto(".u32", "application/octet-stream"),
        new ContentTypeDto(".uls", "text/iuls"),
        new ContentTypeDto(".user", "text/plain"),
        new ContentTypeDto(".ustar", "application/x-ustar"),
        new ContentTypeDto(".vb", "text/plain"),
        new ContentTypeDto(".vbdproj", "text/plain"),
        new ContentTypeDto(".vbk", "video/mpeg"),
        new ContentTypeDto(".vbproj", "text/plain"),
        new ContentTypeDto(".vbs", "text/vbscript"),
        new ContentTypeDto(".vcf", "text/x-vcard"),
        new ContentTypeDto(".vcproj", "Application/xml"),
        new ContentTypeDto(".vcs", "text/plain"),
        new ContentTypeDto(".vcxproj", "Application/xml"),
        new ContentTypeDto(".vddproj", "text/plain"),
        new ContentTypeDto(".vdp", "text/plain"),
        new ContentTypeDto(".vdproj", "text/plain"),
        new ContentTypeDto(".vdx", "application/vnd.ms-visio.viewer"),
        new ContentTypeDto(".vml", "text/xml"),
        new ContentTypeDto(".vscontent", "application/xml"),
        new ContentTypeDto(".vsct", "text/xml"),
        new ContentTypeDto(".vsd", "application/vnd.visio"),
        new ContentTypeDto(".vsi", "application/ms-vsi"),
        new ContentTypeDto(".vsix", "application/vsix"),
        new ContentTypeDto(".vsixlangpack", "text/xml"),
        new ContentTypeDto(".vsixmanifest", "text/xml"),
        new ContentTypeDto(".vsmdi", "application/xml"),
        new ContentTypeDto(".vspscc", "text/plain"),
        new ContentTypeDto(".vss", "application/vnd.visio"),
        new ContentTypeDto(".vsscc", "text/plain"),
        new ContentTypeDto(".vssettings", "text/xml"),
        new ContentTypeDto(".vssscc", "text/plain"),
        new ContentTypeDto(".vst", "application/vnd.visio"),
        new ContentTypeDto(".vstemplate", "text/xml"),
        new ContentTypeDto(".vsto", "application/x-ms-vsto"),
        new ContentTypeDto(".vsw", "application/vnd.visio"),
        new ContentTypeDto(".vsx", "application/vnd.visio"),
        new ContentTypeDto(".vtx", "application/vnd.visio"),
        new ContentTypeDto(".wav", "audio/wav"),
        new ContentTypeDto(".wave", "audio/wav"),
        new ContentTypeDto(".wax", "audio/x-ms-wax"),
        new ContentTypeDto(".wbk", "application/msword"),
        new ContentTypeDto(".wbmp", "image/vnd.wap.wbmp"),
        new ContentTypeDto(".wcm", "application/vnd.ms-works"),
        new ContentTypeDto(".wdb", "application/vnd.ms-works"),
        new ContentTypeDto(".wdp", "image/vnd.ms-photo"),
        new ContentTypeDto(".webarchive", "application/x-safari-webarchive"),
        new ContentTypeDto(".webtest", "application/xml"),
        new ContentTypeDto(".wiq", "application/xml"),
        new ContentTypeDto(".wiz", "application/msword"),
        new ContentTypeDto(".wks", "application/vnd.ms-works"),
        new ContentTypeDto(".WLMP", "application/wlmoviemaker"),
        new ContentTypeDto(".wlpginstall", "application/x-wlpg-detect"),
        new ContentTypeDto(".wlpginstall3", "application/x-wlpg3-detect"),
        new ContentTypeDto(".wm", "video/x-ms-wm"),
        new ContentTypeDto(".wma", "audio/x-ms-wma"),
        new ContentTypeDto(".wmd", "application/x-ms-wmd"),
        new ContentTypeDto(".wmf", "application/x-msmetafile"),
        new ContentTypeDto(".wml", "text/vnd.wap.wml"),
        new ContentTypeDto(".wmlc", "application/vnd.wap.wmlc"),
        new ContentTypeDto(".wmls", "text/vnd.wap.wmlscript"),
        new ContentTypeDto(".wmlsc", "application/vnd.wap.wmlscriptc"),
        new ContentTypeDto(".wmp", "video/x-ms-wmp"),
        new ContentTypeDto(".wmv", "video/x-ms-wmv"),
        new ContentTypeDto(".wmx", "video/x-ms-wmx"),
        new ContentTypeDto(".wmz", "application/x-ms-wmz"),
        new ContentTypeDto(".wpl", "application/vnd.ms-wpl"),
        new ContentTypeDto(".wps", "application/vnd.ms-works"),
        new ContentTypeDto(".wri", "application/x-mswrite"),
        new ContentTypeDto(".wrl", "x-world/x-vrml"),
        new ContentTypeDto(".wrz", "x-world/x-vrml"),
        new ContentTypeDto(".wsc", "text/scriptlet"),
        new ContentTypeDto(".wsdl", "text/xml"),
        new ContentTypeDto(".wvx", "video/x-ms-wvx"),
        new ContentTypeDto(".x", "application/directx"),
        new ContentTypeDto(".xaf", "x-world/x-vrml"),
        new ContentTypeDto(".xaml", "application/xaml+xml"),
        new ContentTypeDto(".xap", "application/x-silverlight-app"),
        new ContentTypeDto(".xbap", "application/x-ms-xbap"),
        new ContentTypeDto(".xbm", "image/x-xbitmap"),
        new ContentTypeDto(".xdr", "text/plain"),
        new ContentTypeDto(".xht", "application/xhtml+xml"),
        new ContentTypeDto(".xhtml", "application/xhtml+xml"),
        new ContentTypeDto(".xla", "application/vnd.ms-excel"),
        new ContentTypeDto(".xlam", "application/vnd.ms-excel.addin.macroEnabled.12"),
        new ContentTypeDto(".xlc", "application/vnd.ms-excel"),
        new ContentTypeDto(".xld", "application/vnd.ms-excel"),
        new ContentTypeDto(".xlk", "application/vnd.ms-excel"),
        new ContentTypeDto(".xll", "application/vnd.ms-excel"),
        new ContentTypeDto(".xlm", "application/vnd.ms-excel"),
        new ContentTypeDto(".xls", "application/vnd.ms-excel"),
        new ContentTypeDto(".xlsb", "application/vnd.ms-excel.sheet.binary.macroEnabled.12"),
        new ContentTypeDto(".xlsm", "application/vnd.ms-excel.sheet.macroEnabled.12"),
        new ContentTypeDto(".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"),
        new ContentTypeDto(".xlt", "application/vnd.ms-excel"),
        new ContentTypeDto(".xltm", "application/vnd.ms-excel.template.macroEnabled.12"),
        new ContentTypeDto(".xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template"),
        new ContentTypeDto(".xlw", "application/vnd.ms-excel"),
        new ContentTypeDto(".xml", "text/xml"),
        new ContentTypeDto(".xmta", "application/xml"),
        new ContentTypeDto(".xof", "x-world/x-vrml"),
        new ContentTypeDto(".XOML", "text/plain"),
        new ContentTypeDto(".xpm", "image/x-xpixmap"),
        new ContentTypeDto(".xps", "application/vnd.ms-xpsdocument"),
        new ContentTypeDto(".xrm-ms", "text/xml"),
        new ContentTypeDto(".xsc", "application/xml"),
        new ContentTypeDto(".xsd", "text/xml"),
        new ContentTypeDto(".xsf", "text/xml"),
        new ContentTypeDto(".xsl", "text/xml"),
        new ContentTypeDto(".xslt", "text/xml"),
        new ContentTypeDto(".xsn", "application/octet-stream"),
        new ContentTypeDto(".xss", "application/xml"),
        new ContentTypeDto(".xtp", "application/octet-stream"),
        new ContentTypeDto(".xwd", "image/x-xwindowdump"),
        new ContentTypeDto(".z", "application/x-compress"),
        new ContentTypeDto(".zip", "application/x-zip-compressed"),
            };
        }
    }
}
