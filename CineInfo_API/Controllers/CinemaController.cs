using AutoMapper;
using CineInfo_API.Data;
using Microsoft.AspNetCore.Mvc;

namespace CineInfo_API.Controllers;

[ApiController]
[Route("[controller]")]
public class CinemaController : Controller {
    private CineInfoContext _dbContext;
    private IMapper _mapper;

    public CinemaController(CineInfoContext dbContext, IMapper mapper) {
        _dbContext = dbContext;
        _mapper = mapper;
    }
}
