using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Charts.Shared.Data.Context;
using Charts.Shared.Logic.Models.File;

namespace Charts.Shared.Logic.File
{
    public interface IFileLogic
    {
        Task<TTSFile> UploadFile(string fileName, byte[] fileBytes);
        Task<object> UpdateFileInformation(List<FileUploadEntry> information);
        Task<TTSFile> GetFileById(Guid id);
        //Task<bool> UpdateDocInformation(DocInformationsInDto model);
        Task<object> DocumentList(Guid applciationId);
        //Task<object> GetDocInformationById(Guid id);
        Task<byte[]> ReadAsync(string filePath);
        Task<bool> ConvertPdf2Pdf(MemoryStream memoryStream, string path);
        Task DeleteFiles(Guid fileId);
        //Task<object> DocumentList(AppFilesInDto appFiles);
        //Task<object> JuristResultFile(Guid id);
        Task DeleteFiles(IEnumerable<Guid> fileIds);
        //Task<object> SetDocInformation(DocInformation model, AgroFile file);

        //Task<Guid> ZipFiles(List<AgroFile> files, string fileName, Guid fileTypeId, string appNumber, Guid? userId,
        //    Guid applicationId);

    }
}
