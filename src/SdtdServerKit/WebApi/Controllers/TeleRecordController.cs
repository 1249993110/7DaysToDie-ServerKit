using IceCoffee.SimpleCRUD.Dtos;
using SdtdServerKit.Data.Entities;
using SdtdServerKit.Data.IRepositories;
using System.ComponentModel.DataAnnotations;

namespace SdtdServerKit.WebApi.Controllers
{
    /// <summary>
    /// 传送记录
    /// </summary>
    [Authorize]
    [RoutePrefix("api/TeleRecord")]
    public class TeleRecordController : ApiController
    {
        private readonly ITeleRecordRepository _repository;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="teleRecordRepository"></param>
        public TeleRecordController(ITeleRecordRepository teleRecordRepository)
        {
            _repository = teleRecordRepository;
        }

        /// <summary>
        /// 分页获取记录
        /// </summary>
        /// <param name="padeNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public async Task<PagedDto<T_TeleRecord>> Get(int padeNumber, int pageSize)
        {
            var data = await _repository.GetPagedListAsync(padeNumber, pageSize);
            return data;
        }

        /// <summary>
        /// 通过Id删除记录
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            int count = await _repository.DeleteByIdAsync(id);
            if (count == 0)
            {
                return NotFound();
            }

            return Ok();
        }

        /// <summary>
        /// 批量删除记录
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("")]
        public async Task<IHttpActionResult> Delete([MinLength(1)] int[] ids)
        {
            int count = await _repository.DeleteByIdsAsync(ids, true);
            return Ok(count);
        }
    }
}