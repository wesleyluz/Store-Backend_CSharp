using lojinha_backend.Business;
using lojinha_backend.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace lojinha_backend.Controller
{
    [ApiController]
    [Route("loja/[controller]")]
    [Authorize("Bearer")]
    public class ProdutoController : ControllerBase
    {
        private IProdutoBusiness _produtoBusiness;
        public ProdutoController(IProdutoBusiness produtoBusiness) 
        {
            _produtoBusiness = produtoBusiness;
        }

        [HttpPost()]
        [ProducesResponseType((200), Type = typeof(Produto))]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)] 
        [ProducesResponseType(401)]
        public IActionResult Create([FromBody] Produto produto) 
        {
            if (produto == null) return BadRequest();
            return Ok(_produtoBusiness.Create(produto));
        }

        [HttpGet("{produtoid}")]
        [ProducesResponseType((200), Type = typeof(Produto))]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public IActionResult GetbyId(int produtoid) 
        {
            var produto = _produtoBusiness.FindById(produtoid);
            if (produto == null) return NotFound();
            return Ok(produto);
        }

        [HttpGet()]
        [ProducesResponseType((200), Type = typeof(Produto))]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public IActionResult GetByName(
            [FromQuery] string name,
            [FromQuery] string sortDirection,
            [FromQuery] int pageSize,
            [FromQuery] int currpage
            ) 
        {
            var produtos = _produtoBusiness.FindByName(name, sortDirection, pageSize, currpage);
            if(produtos == null) return BadRequest();
            return Ok(produtos);

        }

        [HttpGet("findAll")]
        [ProducesResponseType((200), Type = typeof(Produto))]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public IActionResult GetAll() 
        {
            var produtos = _produtoBusiness.FindAll();
            if(produtos == null) return NotFound();
            return Ok(produtos);
        }
        [HttpPut]
        [ProducesResponseType((200), Type = typeof(Produto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public IActionResult Update([FromBody] Produto produto) 
        {
            if (produto == null) return BadRequest();
            return Ok(_produtoBusiness.Update(produto));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType((204))]
        [ProducesResponseType((400))]
        [ProducesResponseType((401))]
        public IActionResult Delete(int id)
        {
            if(_produtoBusiness.Delete(id)) return NoContent();
            return BadRequest();
        }

    }
}
