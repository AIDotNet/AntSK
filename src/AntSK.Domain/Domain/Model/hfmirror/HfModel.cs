using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntSK.Domain.Domain.Model.hfmirror
{
    public class HfModel
    {
        public List<HfModels> models { get; set; }
        public int numItemsPerPage { get; set; }
        public int numTotalItems { get; set; }
        public int pageIndex { get; set; }
    }
    public class HfModels
    {
        public string Author { get; set; }
        public HfAuthorData AuthorData { get; set; }
        public int Downloads { get; set; }
        public bool Gated { get; set; }
        public string Id { get; set; }
        public DateTime LastModified { get; set; }
        public int Likes { get; set; }
        public string PipelineTag { get; set; }
        public bool Private { get; set; }
        public string RepoType { get; set; }
        public bool IsLikedByUser { get; set; }
    }

    public class HfAuthorData
    {
        public string AvatarUrl { get; set; }
        public string Fullname { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsHf { get; set; }
        public bool IsEnterprise { get; set; }
    }
}
