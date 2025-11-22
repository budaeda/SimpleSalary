using Rocket.API.Collections;
using Rocket.API.Serialisation;
using Rocket.Core;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static SimpleSalary.SimpleSalaryConfiguration;
using Logger = Rocket.Core.Logging.Logger;

namespace SimpleSalary
{
    public class SimpleSalaryMain : RocketPlugin<SimpleSalaryConfiguration>
    {
        protected override void Load()
        {
            InvokeRepeating("Event_OnSalaryInterval", Configuration.Instance.SalaryPayoutInterval, Configuration.Instance.SalaryPayoutInterval);
            Logger.Log($"{Assembly.GetName().Name} {Assembly.GetName().Version} Has Loaded");
        }

        protected override void Unload()
        {
            CancelInvoke("Event_OnSalaryInterval");
            Logger.Log($"{Assembly.GetName().Name} {Assembly.GetName().Version} Has Unloaded");
        }

        private void Event_OnSalaryInterval()
        {
            foreach (SteamPlayer Player in Provider.clients)
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromSteamPlayer(Player);

                HashSet<string> JobNames = new HashSet<string>();
                uint TotalPayout = 0;

                foreach (SalaryInfo salaryInfo in Configuration.Instance.SalaryInfoList)
                {
                    bool hasPermission = HasPermissionGroup(uPlayer, salaryInfo.PermissionsGroupId);
                    if (!hasPermission) continue;

                    RocketPermissionsGroup group = R.Permissions.GetGroup(salaryInfo.PermissionsGroupId);
                    JobNames.Add(group.DisplayName);

                    TotalPayout += salaryInfo.SalaryPayout; 
                }

                if (TotalPayout == 0) continue;

                string JobsString = string.Join(", ", JobNames);
                uPlayer.Experience += TotalPayout;

                UnturnedChat.Say(uPlayer, Translate("SalaryReceived", TotalPayout, JobsString), Color.yellow);
            }
        }

        private bool HasPermissionGroup(UnturnedPlayer uPlayer, string groupId)
        {
            return R.Permissions.GetGroups(uPlayer, false)
               .Any(g => g.Id.Equals(groupId, StringComparison.OrdinalIgnoreCase));
        }

        public override TranslationList DefaultTranslations
        {
            get
            {
                TranslationList translationList = new TranslationList();
                translationList.Add("SalaryReceived", "You Have Received A Total Of ${0} For Your Work As A {1}");

                return translationList;
            }
        }
    }
}
