using IceCoffee.SimpleCRUD;
using SdtdServerKit.Data.Entities;
using SdtdServerKit.Data.IRepositories;
using System.ComponentModel.DataAnnotations;

namespace SdtdServerKit.WebApi.Controllers
{
    /// <summary>
    /// 礼品
    /// </summary>
    [Authorize]
    [RoutePrefix("api/VipGift")]
    public class VipGiftController : ApiController
    {
        private readonly IVipGiftRepository _vipGiftRepository;
        private readonly IItemListRepository _itemListRepository;
        private readonly ICommandListRepository _commandListRepository;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="vipGiftRepository"></param>
        /// <param name="itemListRepository"></param>
        /// <param name="commandListRepository"></param>
        public VipGiftController(IVipGiftRepository vipGiftRepository, IItemListRepository itemListRepository, ICommandListRepository commandListRepository)
        {
            _vipGiftRepository = vipGiftRepository;
            _itemListRepository = itemListRepository;
            _commandListRepository = commandListRepository;
        }

        /// <summary>
        /// 通过Id获取记录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        [ResponseType(typeof(T_VipGift))]
        public async Task<IHttpActionResult> Get(string id)
        {
            var entity = await _vipGiftRepository.GetByIdAsync(id);
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
        public async Task<IEnumerable<T_VipGift>> Get()
        {
            return await _vipGiftRepository.GetAllAsync();
        }

        /// <summary>
        /// 新增记录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post([FromBody] VipGift model)
        {
            var entity = new T_VipGift()
            {
                Id = model.Id,
                Name = model.Name,
                CreatedAt = DateTime.Now,
                ClaimState = model.ClaimState,
                TotalClaimCount = model.TotalClaimCount,
                Description = model.Description,
            };
            await _vipGiftRepository.InsertAsync(entity);
            return Ok();
        }


        /// <summary>
        /// 通过Id更新记录
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(string id, [FromBody] VipGift model)
        {
            var entity = await _vipGiftRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            entity.Name = model.Name;
            entity.ClaimState = model.ClaimState;
            entity.TotalClaimCount = model.TotalClaimCount;
            entity.Description = model.Description;
           
            await _vipGiftRepository.UpdateAsync(entity);
            return Ok();
        }

        /// <summary>
        /// 通过Id删除记录
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            int count = await _vipGiftRepository.DeleteByIdAsync(id);
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
        /// <param name="deleteAll"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("")]
        public async Task<IHttpActionResult> Delete([FromUri] string[]? ids, [FromUri] bool deleteAll = false)
        {
            int count = 0;

            if (deleteAll)
            {
               count = await _vipGiftRepository.DeleteAllAsync(true);
            }
            else if(ids != null && ids.Length > 0)
            {
                count = await _vipGiftRepository.DeleteByIdsAsync(ids, true);
            }

            return Ok(count);
        }

        /// <summary>
        /// 获取礼品关联的物品清单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/Items")]
        public async Task<IEnumerable<T_ItemList>> GetItems(string id)
        {
            var data = await _itemListRepository.GetListByVipGiftIdAsync(id);
            return data;
        }

        /// <summary>
        /// 修改礼品关联的物品
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemIds"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}/Items")]
        public async Task<IHttpActionResult> PutItems(string id, [FromBody, Required] int[] itemIds)
        {
            var entity = await _vipGiftRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            var entities = new List<T_VipGiftItem>();
            foreach (var item in itemIds)
            {
                entities.Add(new T_VipGiftItem()
                {
                    VipGiftId = id,
                    ItemId = item
                });
            }

            using var unitOfWork = ModApi.ServiceContainer.Resolve<IUnitOfWorkFactory>().Create();
            var userTagRepository = unitOfWork.GetRepository<IVipGiftItemRepository>();
            await userTagRepository.DeleteByVipGiftIdAsync(id);
            await userTagRepository.InsertAsync(entities);
            unitOfWork.Commit();

            return Ok();
        }

        /// <summary>
        /// 获取礼品关联的命令Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/Commands")]
        public async Task<IEnumerable<T_CommandList>> GetCommands(string id)
        {
            var data = await _commandListRepository.GetListByVipGiftIdAsync(id);
            return data;
        }

        /// <summary>
        /// 修改礼品关联的命令
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemIds"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}/Commands")]
        public async Task<IHttpActionResult> PutCommands(string id, [FromBody, Required] int[] itemIds)
        {
            var entity = await _vipGiftRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            var entities = new List<T_VipGiftCommand>();
            foreach (var item in itemIds)
            {
                entities.Add(new T_VipGiftCommand()
                {
                    VipGiftId = id,
                    CommandId = item
                });
            }

            using var unitOfWork = ModApi.ServiceContainer.Resolve<IUnitOfWorkFactory>().Create();
            var userTagRepository = unitOfWork.GetRepository<IVipGiftCommandRepository>();
            await userTagRepository.DeleteByVipGiftIdAsync(id);
            await userTagRepository.InsertAsync(entities);
            unitOfWork.Commit();

            return Ok();
        }
    }
}