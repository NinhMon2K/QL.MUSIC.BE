using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QL.MUSIC.BL;
using QL.MUSIC.Common.Entities;

namespace QL.MUSIC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongsController : BasesController<song>
    {
        public SongsController(IBaseBL<song> baseBL) : base(baseBL)
        {
        }
    }
}
