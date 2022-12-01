using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QL.MUSIC.BL;
using QL.MUSIC.Common.Entities;

namespace QL.MUSIC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlbumsController : BasesController<album>
    {
        public AlbumsController(IBaseBL<album> baseBL) : base(baseBL)
        {
        }
    }
}
