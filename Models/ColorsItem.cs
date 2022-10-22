//using Microsoft.Azure.Cosmos.Table;

namespace ColorsAPI.Models
{
    public class ColorsItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Data { get; set; }

    }

    //public class ColorsItemEntity : TableEntity
    //{

    //    public ColorsItemEntity()
    //    {
    //    }
    //    public ColorsItemEntity(string Thingid)
    //    {
    //        PartitionKey = "Thing";
    //        RowKey = Thingid;
    //    }

    //    public int Id { get; set; }
    //    public string Name { get; set; }

    //}

}
