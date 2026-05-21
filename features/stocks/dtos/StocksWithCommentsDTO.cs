using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _1.first_learn.models;

namespace _1.first_learn.features.stocks.dtos;
    public class StocksWithCommentsDTO
    {
         public int Id {get; set;}

        public string Symbol {get; set;} = string.Empty;

        public string CompanyName { get; set; } = string.Empty;

        public decimal Purchase {get; set;}

        public decimal Divdent {get; set;}

        public string Industry { get; set; } = string.Empty;

        public long Marketcap { get; set; }

        public List<CommentDTO> Comments {get; set;} = [];
    }