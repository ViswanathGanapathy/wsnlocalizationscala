﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Globalization;

using Scala.Core;

namespace Elab.Rtls.Engines.WsnEngine
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, AutomaticSessionShutdown = false, IncludeExceptionDetailInFaults = true)]
    public class WsnEngineService : Scala.Core.IQueryService, IMapService, IEventService, ITagInformationService
    {
        public WsnEngineService()
        {
            this.WsnEngine = WsnEngine.Instance;
            this.WsnEngine.EventRaised += this.WsnEngineEventRaised;
            EventIDs = new List<string>();
        }
        
        public WsnEngine WsnEngine
        {
            get;
            set;
        }

        private IEventSourceCallback Callback
        {
            get;
            set;
        }

        private List<string> EventIDs;

        /// <summary>
        /// Queries the Engine to retrieve tag information
        /// </summary>
        /// <param name="query"><see cref="Core.Query"/></param>
        /// <returns></returns>
        public QueryResponse Query(Query query)
        {
            return this.WsnEngine.Query(query);
        }    

        /// <summary>
        /// Sends a trivial word to a service and returns a trivial phrase.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public string Ping(string word)
        {
            return this.WsnEngine.Ping(word);
        }

        private void WsnEngineEventRaised(object sender, EventMessage eventMessage)
        {
            //this.Logger.Trace("Event received from Ekahau4EngineAdapter, triggering callback.");
            if (this.Callback == null)
            {
                //this.Logger.Info("Callback is null!");
            }
            else
            {
                if (EventIDs.Contains(eventMessage.EventSubscriptionId))
                    this.Callback.OnEventRaised(eventMessage);
            }
        }

        #region ITagInformationService Members

        public List<Tag> GetAllTags()
        {
            return this.WsnEngine.GetAllTags();
        }

        public Tag GetTag(string tagId)
        {
            return this.WsnEngine.GetTag(tagId);
        }

        #endregion

        #region IMapService Members

        public List<Site> GetAllSites()
        {
            throw new FaultException(
                new FaultReason(
                    new FaultReasonText(
                        "Sites are not supported without a middleware.", CultureInfo.InvariantCulture)));

        }

        public void SaveSite(Site site)
        {
            throw new FaultException(
               new FaultReason(
                   new FaultReasonText(
                       "Sites are not supported without a middleware.", CultureInfo.InvariantCulture)));

        }

        public Map AssociateMiddlewareMapToEngineMaps(MiddlewareToEngineMapLink mapObject)
        {
            throw new FaultException(
                new FaultReason(
                    new FaultReasonText(
                        "Associating middleware map with enginemaps is not supported without a middleware.", CultureInfo.InvariantCulture)));

        }

        public void DeleteMap(string mapId)
        {
            throw new FaultException(
                new FaultReason(
                    new FaultReasonText(
                        "Deleting a map is not supported without a middleware.", CultureInfo.InvariantCulture)));

        }

        public void DeleteSite(string siteId)
        {
            throw new FaultException(
                new FaultReason(
                    new FaultReasonText(
                        "Sites are not supported without a middleware.", CultureInfo.InvariantCulture)));

        }

        public void SaveMap(Map map)
        {
            throw new FaultException(
                new FaultReason(
                    new FaultReasonText(
                        "Saving of a map is not supported without a middleware.", CultureInfo.InvariantCulture)));

        }

        public List<Map> GetAllMaps()
        {
            return this.WsnEngine.GetAllMaps();
        }

        public List<Zone> GetAllZones()
        {
            return this.WsnEngine.GetAllZones();
        }

        public Map GetMap(string mapId)
        {
            return this.WsnEngine.GetMap(mapId);

        }

        public Zone GetZone(string zoneId)
        {
            return this.WsnEngine.GetZone(zoneId);
        }

        #endregion

        #region IEventService Members

        public void UnsubscribeAll()
        {
            this.WsnEngine.UnsubscribeAll();
            this.EventIDs.Clear();
        }

        public void Subscribe(EventSubscription eventSubscription)
        {
            this.Callback = OperationContext.Current.GetCallbackChannel<IEventSourceCallback>();
            this.WsnEngine.Subscribe(eventSubscription);
            this.EventIDs.Add(eventSubscription.EventId);
        }

        public void Unsubscribe(string id)
        {
            this.WsnEngine.Unsubscribe(id);
            this.EventIDs.Remove(id);
        }  

        #endregion
    }
}
