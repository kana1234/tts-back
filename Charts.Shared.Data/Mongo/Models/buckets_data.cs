using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Charts.Shared.Data.Mongo.Models
{
    [BsonIgnoreExtraElements]
    public class buckets_data
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement(nameof(bucket))]
        public string bucket { get; set; }

        [BsonElement(nameof(ts))]
        public DateTime? ts { get; set; }

        [BsonElement(nameof(val))]
        public Value val { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class Value
    {
        //[BsonElement(nameof(hMode))]
        //public int? hMode { get; set; }

        [BsonElement(nameof(t1))]
        public decimal? t1 { get; set; }

        [BsonElement(nameof(t2))]
        public decimal? t2 { get; set; }

        [BsonElement(nameof(t3))]
        public decimal? t3 { get; set; }

        //[BsonElement(nameof(t4))]
        //public decimal? t4 { get; set; }

        //[BsonElement(nameof(t5))]
        //public decimal? t5 { get; set; }

        //[BsonElement(nameof(t6))]
        //public decimal? t6 { get; set; }

        [BsonElement(nameof(p1))]
        public decimal? p1 { get; set; }

        [BsonElement(nameof(p2))]
        public decimal? p2 { get; set; }

        //[BsonElement(nameof(tBH))]
        //public int? tBH { get; set; }

        //[BsonElement(nameof(tHome))]
        //public int? tHome { get; set; }

        [BsonElement(nameof(vlv1))]
        public int? vlv1 { get; set; }

        [BsonElement(nameof(vlv2))]
        public int? vlv2 { get; set; }

        [BsonElement(nameof(vlv3))]
        public int? vlv3 { get; set; }

        [BsonElement(nameof(vlv4))]
        public int? vlv4 { get; set; }

        [BsonElement(nameof(vlv5))]
        public int? vlv5 { get; set; }

        //[BsonElement(nameof(vlv6))]
        //public int? vlv6 { get; set; }

        //[BsonElement(nameof(flowNow))]
        //public decimal? flowNow { get; set; }

        //[BsonElement(nameof(flowNow2))]
        //public decimal? flowNow2 { get; set; }

        //[BsonElement(nameof(gcal))]
        //public int? gcal { get; set; }

        //[BsonElement(nameof(rstcnt))]
        //public int? rstcnt { get; set; }

        //[BsonElement(nameof(tRTC))]
        //public decimal? tRTC { get; set; }

        //[BsonElement(nameof(tenge))]
        //public int? tenge { get; set; }

        [BsonElement(nameof(VQuality))]
        public int? VQuality { get; set; }
    }
}
