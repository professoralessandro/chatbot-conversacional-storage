#region REFERENCE
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
#endregion REFERENCE

namespace ChatbotConversacionalStorage.Application.Helper.Static.File
{
    public class FileHelper
    {
        #region PUBLIC METHOD
        public static string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return string.Concat(Path.GetFileNameWithoutExtension(fileName)
                                , "_"
                                , Guid.NewGuid().ToString().AsSpan(0, 4)
                                , Path.GetExtension(fileName));
        }

        public static string GetFullFilePath(string fileName)
        {
            // Obtém o diretório atual
            string currentDirectory = Directory.GetCurrentDirectory();

            // Combina o diretório atual com o nome do arquivo para obter o caminho completo
            string fullPath = Path.Combine(currentDirectory, fileName);

            return fullPath;
        }

        public static string GetCurrentDirectoryPath()
        {
            // Obtém o diretório atual
            return Directory.GetCurrentDirectory();
        }

        public static string RemoveIncorrectFolderAndReturnFilePath(string filePath, string filePathFromApi)
        {
            // Obtém o diretório do arquivo
            string directoryPath = Path.GetDirectoryName(filePath);

            // Obtém o diretório pai do diretório atual
            string parentDirectory = Directory.GetParent(directoryPath)?.FullName;

            // Adicionando pasta da solucao aplication combinando com o caminho da pasta aonde esta o arquivo
            parentDirectory = string.Concat(parentDirectory, "\\ChatbotConversacionalNotifications.Application", filePathFromApi);

            return parentDirectory;
        }

        public static string ReturnImageFileExtention(IFormFile file)
        {
            try
            {
                if (file.ContentType.Contains("jpeg"))
                {
                    return ".jpeg";
                }
                else if (file.ContentType.Contains("jpg"))
                {
                    return ".jpg";
                }
                else if (file.ContentType.Contains("png"))
                {
                    return ".png";
                }
                else if (file.ContentType.Contains("gif"))
                {
                    return ".gif";
                }
                else
                {
                    throw new ValidationException("Unknown image file");
                }
            }
            catch (ValidationException ex)
            {
                throw new ValidationException($"Erro ao processar o arquivo: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao processar o arquivo: " + ex.Message);
            }
        }

        public static IFormFile ConvertBase64ToImage(string imageStringBase64, string imageName)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(imageStringBase64.Split(",")[1]);
                MemoryStream stream = new MemoryStream(bytes);
                IFormFile file = new FormFile(stream, 0, bytes.Length, imageName, imageName);
                return file;
            }
            catch (Exception ex)
            {
                throw new ValidationException(ex.Message);
            }
            
        }

        public static string GetFileTypeFromBase64(string base64String)
        {
            // Converte a string base64 para um array de bytes
            byte[] fileBytes = Convert.FromBase64String(base64String.Split(",")[1]);

            // Usa um MemoryStream para ler os bytes
            using (MemoryStream ms = new MemoryStream(fileBytes))
            {
                // Usa o método GetFileType para determinar o tipo do arquivo
                var fileType = GetFileType(ms);

                if (!IsImageFile(fileType)) throw new ValidationException("Image type is not valid");

                return fileType;
            }
        }
        #endregion PUBLIC METHOD

        #region PRIVATE
        private static string GetFileType(Stream fileStream)
        {
            // Lê os primeiros bytes do arquivo para determinar o tipo
            byte[] buffer = new byte[256];
            fileStream.Read(buffer, 0, buffer.Length);

            // Verifica os bytes iniciais para identificar o tipo do arquivo
            if (buffer[0] == 0xFF && buffer[1] == 0xD8)
            {
                return "image/jpeg";
            }
            else if (buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47)
            {
                return "image/png";
            }
            else if (buffer[0] == 0x25 && buffer[1] == 0x50 && buffer[2] == 0x44 && buffer[3] == 0x46)
            {
                return "application/pdf";
            }
            else if (buffer[0] == 0x50 && buffer[1] == 0x4B && buffer[2] == 0x03 && buffer[3] == 0x04)
            {
                return "application/zip";
            }
            else
            {
                return "unknown";
            }
        }

        public static bool IsImageFile(string fileType)
        {
            if (fileType == "image/jpeg" || fileType == "image/png" || fileType == "image/jpeg")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion PRIVATE
    }
}
