using System;
using System.Collections.Generic;
using System.Text;
using MarketAnalyzer.DataAccess;
using MarketAnalyzer.Data;
using System.Threading.Tasks;

namespace MarketAnalyzer.Business
{
    public class UserSettingsBO
    {
        public async Task<MultiplierSetting> SetMultiplierSettings(MultiplierSetting multiplierSetting)
        {
            var genericDao = new GenericDao<MultiplierSetting>();


            var setting = genericDao.GetSingleBySync(x => x.UserId == multiplierSetting.UserId);

            if(setting== null)
            {
                var newSettingAdded = await genericDao.AddAsync(multiplierSetting);
                return newSettingAdded;
            }
            else
            {
                multiplierSetting.Id = setting.Id;
                var updatedSetting = await genericDao.UpdateAsync(multiplierSetting);

                return updatedSetting;


            }
        }

        public MultiplierSetting GetMultiplierSetting(string UserId)
        {
            var genericDao = new GenericDao<MultiplierSetting>();

            var setting = genericDao.GetSingleBySync(x => x.UserId == UserId);

            if(setting != null)
            {
                return setting;
            }
            else
            {
                return null;
            }
        }

    }
}
