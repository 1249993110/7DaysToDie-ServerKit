using SdtdServerKit.Data.Entities;
using SdtdServerKit.Data.IRepositories;
using System.ComponentModel.DataAnnotations;

namespace SdtdServerKit.WebApi.Controllers
{
    /// <summary>
    /// 商品
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Goods")]
    public class GoodsController : ApiController
    {
        private readonly IGoodsRepository _repository;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="repository"></param>
        public GoodsController(IGoodsRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// 通过Id获取记录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{id:int}")]
        [ResponseType(typeof(T_Goods))]
        public async Task<IHttpActionResult> Get(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            return Ok(entity);
        }

        /// <summary>
        /// 获取所有记录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public async Task<IEnumerable<T_Goods>> Get()
        {
            return await _repository.GetAllOrderByIdAsync();
        }

        /// <summary>
        /// 新增记录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post([FromBody] Goods model)
        {
            var entity = new T_Goods()
            {
                Id = model.Id,
                Name = model.Name,
                CreatedAt = DateTime.Now,
                Content = model.Content,
                ContentType = model.ContentType,
                InMainThread = model.InMainThread,
                Price = model.Price
            };
            await _repository.InsertAsync(entity);
            return Ok();
        }


        /// <summary>
        /// 通过Id更新记录
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> Put(int id, [FromBody] Goods model)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            entity.Name = model.Name;
            entity.Content = model.Content;
            entity.ContentType = model.ContentType;
            entity.InMainThread = model.InMainThread;
            entity.Price = model.Price;
           
            await _repository.UpdateAsync(entity);
            return Ok();
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