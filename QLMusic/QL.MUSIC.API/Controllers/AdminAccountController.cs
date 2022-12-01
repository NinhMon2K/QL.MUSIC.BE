using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QL.MUSIC.BL;
using QL.MUSIC.Common.Entities;
using QL.MUSIC.Common.Entities.Data;

namespace QL.MUSIC.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AdminAccountController : BasesController<adminaccount>
    {
        public AdminAccountController(IBaseBL<adminaccount> baseBL) : base(baseBL)
        {
        }
    }
}
