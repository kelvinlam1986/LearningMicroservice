using Infrastructure.Common.Models;
using Inventory.Product.API.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.Inventory;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Inventory.Product.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [Route("items/{itemNo}", Name = "GetAllByItemNo")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<InventoryEntryDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<InventoryEntryDto>>> GetAllByItemNo([Required] string itemNo)
        {
            var results = await _inventoryService.GetAllByItemNo(itemNo);
            return Ok(results);
        }

        [Route("items/{itemNo}/paging", Name = "GetAllByItemNoPaging")]
        [HttpGet]
        [ProducesResponseType(typeof(PagedList<InventoryEntryDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PagedList<InventoryEntryDto>>> GetAllByItemNoPaging(
            [Required] string itemNo,
            [FromQuery] GetInventoryPagingQuery query)
        {
            query.SetItemNo(itemNo);
            var results = await _inventoryService.GetAllByItemNoPaging(query);
            return Ok(results);
        }

        [Route("{id}", Name = "GetInventoryById")]
        [HttpGet]
        [ProducesResponseType(typeof(InventoryEntryDto), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<InventoryEntryDto>> GetInventoryById(
          [Required] string id)
        {
            var results = await _inventoryService.GetByIdAsync(id);
            return Ok(results);
        }

        [HttpPost("purchase/{itemNo}")]
        [ProducesResponseType(typeof(InventoryEntryDto), (int)HttpStatusCode.NoContent)]
        public async Task<ActionResult<InventoryEntryDto>> PurchaseOrder(
            [Required] string itemNo,
            [FromBody] PurchaseProductDto model)
        {
            var results = await _inventoryService.PurchaseItemAsync(itemNo, model);
            return Ok(results);
        }

        [HttpPost("sales/{itemNo}")]
        [ProducesResponseType(typeof(InventoryEntryDto), (int)HttpStatusCode.NoContent)]
        public async Task<ActionResult<InventoryEntryDto>> SalesItem(
          [Required] string itemNo,
          [FromBody] SalesProductDto model)
        {
            var results = await _inventoryService.SalesItemAsync(itemNo, model);
            return Ok(results);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(InventoryEntryDto), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<InventoryEntryDto>> DeleteById(
          [Required] string id)
        {
            var entity = await _inventoryService.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            await _inventoryService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("sales/order-no/{orderNo}", Name = "SalesOrder")]
        [ProducesResponseType(typeof(CreatedSalesOrderSuccessDto), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CreatedSalesOrderSuccessDto>> SalesOrder([Required]string orderNo, [FromBody] SalesOrderDto model)
        {
            model.OrderNo = orderNo;
            var documentNo = await _inventoryService.SalesOrderAsync(model);
            var result = new CreatedSalesOrderSuccessDto(documentNo);
            return Ok(result);  
        }
    }
}
