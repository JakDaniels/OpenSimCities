/*
 * Copyright (c) Contributors, Jak Daniels 
 * See CONTRIBUTORS.TXT for a full list of copyright holders.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the OpenSimulator Project nor the
 *       names of its contributors may be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE DEVELOPERS ``AS IS'' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE CONTRIBUTORS BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
using System;
using System.Timers;
using System.Net;
using System.IO;
using System.Text;
using System.Collections.Generic;
using OpenMetaverse;
using Nini.Config;
using System.Threading;
using log4net;
using OpenSim.Region.Framework.Interfaces;
using OpenSim.Region.Framework.Scenes;
using OpenSim.Framework;
using OpenSim.Framework.Console;
using Mono.Addins;

[assembly: Addin("OpenSimCitiesModule", "0.1")]
[assembly: AddinDependency("OpenSim.Region.Framework", OpenSim.VersionInfo.VersionNumber)]

namespace CitiesModule
{
    [Extension(Path = "/OpenSim/RegionModules", NodeName = "RegionModule", Id = "OpenSimCities")]
    public class OpenSimCities : INonSharedRegionModule
    {
        #region Fields
        
        public Scene m_scene;
        public IConfigSource m_config;
        public RegionInfo m_regionInfo;
        public string m_name = "OpenSimCitiesModule";
        
        private bool m_enabled = false;
        private bool m_ready = false;
        private uint m_frame = 0;
        private Vector3 m_shoutPos = new Vector3(128f, 128f, 30f);
        
        #endregion
        #region IRegionModuleBase implementation

        public string Name { get { return m_name; } }
        public Type ReplaceableInterface { get { return null; } }

        public void Initialise (IConfigSource source)
        {
            m_config = source;
        }


        public void Close ()
        {
            if (m_enabled) {
                    //m_scene.EventManager.OnFrame -= CitiesUpdate;
            }
        }


        public void AddRegion (Scene scene)
        {
            m_log.InfoFormat("[{0}]: Adding region '{1}' to this module", m_name, scene.RegionInfo.RegionName);
            IConfig cnf = m_config.Configs[scene.RegionInfo.RegionName];
            
            if(cnf == null)
            {
                m_log.InfoFormat("[{0}]: No region section [{1}] found in configuration. Module in this region is set to Disabled", m_name, scene.RegionInfo.RegionName);
                m_enabled = false;
                return;
            }
              
            m_enabled = cnf.GetBoolean("CitiesEnabled", false);
            
            if (m_enabled)
            {
		//get config here
                m_frame = 0;
                m_ready = true; // Mark Module Ready for duty
                m_shoutPos = new Vector3(scene.RegionInfo.RegionSizeX / 2f, scene.RegionInfo.RegionSizeY / 2f, 30f);
                //scene.EventManager.OnFrame += CitiesUpdate;
                m_scene = scene;
            }
            else
            {
                m_log.InfoFormat("[{0}]: Module in this region is set to Disabled", m_name);
            }
        }

        public void RemoveRegion (Scene scene)
        {
            m_log.InfoFormat("[{0}]: Removing region '{1}' from this module", m_name, scene.RegionInfo.RegionName);
            if (m_enabled)
            {
                //scene.EventManager.OnFrame -= CitiesUpdate;
            }
        }


        public void RegionLoaded (Scene scene)
        {

        }

        #endregion

        #region CitiesModule
        public void CitiesUpdate()
        {
              	if (((m_frame++ % m_frameUpdateRate) != 0) || !m_ready) {
                return;
        }
        #endregion
        
        