using Diffing_API.IRepository;
using Diffing_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Diffing_API.Controllers
{
    [Route("v1/diff")]
    [ApiController]
    public class DiffingController : ControllerBase
    {
        private IDiffRepository _repository;

        public DiffingController(IDiffRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetDiff([FromRoute] int id)
        {
            DiffModel diff = await _repository.GetData(id);

            if (diff != null && diff.left != null && diff.right != null)
            {
                return Ok(diff.left.Compare(diff.right).Value);
            }
            else
            {
                return NotFound("");
            }
        } 

        [HttpPut]
        [Route("{id}/left")]
        public async Task<IActionResult> PutLeft([FromRoute] int id, [FromBody] RequestData reqData)
        {
            if (reqData.data.Equals("null"))
            {
                return BadRequest("");
            }

            DiffModel diff = await _repository.GetData(id);
            DiffModel newDiff;

            if (diff != null)
            {
                newDiff = diff;
                newDiff.left = reqData;
            }
            else
            {
                newDiff = new DiffModel(id);
                newDiff.left = reqData;
            }

            await _repository.PostData(newDiff);

            return Created("", null);
        }

        [HttpPut]
        [Route("{id}/right")]
        public async Task<IActionResult> PutRight([FromRoute] int id,  [FromBody] RequestData reqData)
        {
            if (reqData.data.Equals("null"))
            {
                return BadRequest("");
            }

            DiffModel diff = await _repository.GetData(id);
            DiffModel newDiff;

            if (diff != null)
            {
                newDiff = diff;
                newDiff.right = reqData;
            }
            else
            {
                newDiff = new DiffModel(id);
                newDiff.right = reqData;
            }

            await _repository.PostData(newDiff);

            return Created("", null);
        }

    }
}
