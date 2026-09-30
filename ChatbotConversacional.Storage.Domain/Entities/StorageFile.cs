#region REFERENCES
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
#endregion REFERENCES

namespace ChatbotConversacionalStorage.Domain.Entities
{
    [Table("storagefiles", Schema = "public")]
    public class StorageFile
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key, Column("fileid")]
        public Guid FileId { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("filepath")]
        public string FilePath { get; set; }

        [Column("Description")]
        public string Description { get; set; }

        [Column("mainfile")]
        public bool MainFile { get; set; }

        [Column("public")]
        public bool Public { get; set; }

        [Column("externalreferenceid")]
        public Guid ExternalReferenceId { get; set; }

        [Column("useraddedid")]
        public Guid UserAddedId { get; set; }

        [Column("userupdatedid")]
        public Guid? UserUpdatedId { get; set; }

        [Column("dateadded", TypeName = "timestamp(6)")]
        public DateTime DateAdded { get; set; }

        [Column("dateupdated", TypeName = "timestamp(6)")]
        public DateTime? DateUpdated { get; set; }
    }
}
