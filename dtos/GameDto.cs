using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _1.first_learn.dtos;
    public record class GameDto(
        int Id,
        string Name,
        string Genre,
        decimal Price, //kiểu decimal thêm hậu tố M
        DateOnly ReleaseDate
    );