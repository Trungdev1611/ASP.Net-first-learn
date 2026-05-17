using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace _1.first_learn.models;

    public class Stock
    {
        public int Id {get; set;}

        public string Symbol {get; set;} = string.Empty;

        public string CompanyName { get; set; } = string.Empty;

        [Column(TypeName ="numeric(18,2)")] // Đổi decimal thành numeric cho đúng chuẩn Postgres
        public decimal Purchase {get; set;}

        [Column(TypeName ="numeric(18,2)")] // Đổi decimal thành numeric cho đúng chuẩn Postgres
        public decimal Divdent {get; set;}

        public string Industry { get; set; } = string.Empty;

        public long Marketcap { get; set; }

        public List<Comment> Comments { get; set; } = [];
    }
