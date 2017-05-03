/* 
 * ScaleOut StateServer dedicated expiration event service sample.
 * 
 * Copyright 2017 ScaleOut Software, Inc.
 * 
 * LICENSE AND DISCLAIMER
 * ----------------------
 * This material contains sample programming source code ("Sample Code").
 * ScaleOut Software, Inc. (SSI) grants you a nonexclusive license to compile, 
 * link, run, display, reproduce, and prepare derivative works of 
 * this Sample Code.  The Sample Code has not been thoroughly
 * tested under all conditions.  SSI, therefore, does not guarantee
 * or imply its reliability, serviceability, or function. SSI
 * provides no support services for the Sample Code.
 *
 * All Sample Code contained herein is provided to you "AS IS" without
 * any warranties of any kind. THE IMPLIED WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGMENT ARE EXPRESSLY
 * DISCLAIMED.  SOME JURISDICTIONS DO NOT ALLOW THE EXCLUSION OF IMPLIED
 * WARRANTIES, SO THE ABOVE EXCLUSIONS MAY NOT APPLY TO YOU.  IN NO 
 * EVENT WILL SSI BE LIABLE TO ANY PARTY FOR ANY DIRECT, INDIRECT, 
 * SPECIAL OR OTHER CONSEQUENTIAL DAMAGES FOR ANY USE OF THE SAMPLE CODE
 * INCLUDING, WITHOUT LIMITATION, ANY LOST PROFITS, BUSINESS 
 * INTERRUPTION, LOSS OF PROGRAMS OR OTHER DATA ON YOUR INFORMATION
 * HANDLING SYSTEM OR OTHERWISE, EVEN IF WE ARE EXPRESSLY ADVISED OF
 * THE POSSIBILITY OF SUCH DAMAGES.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

using Soss.Client;

namespace SossEventService
{
    public partial class ExpirationEventService : ServiceBase
    {
        private readonly string _cacheName = "TODO: Your Cache Name here!";

        // NamedCache instance used to access the SOSS datastore:
        private NamedCache _nc = null;

        // Trace source associated with this service:
        private TraceSource _traceSource = new TraceSource("ExpirationEventService", defaultLevel: SourceLevels.Off);

        /// <summary>
        /// Constructor. Typically should not be modified.
        /// Use OnStart override below to handle initialization of your service.
        /// </summary>
        public ExpirationEventService()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initialize the service.
        /// </summary>
        /// <param name="args">Data passed by the SCM's start command.</param>
        protected override void OnStart(string[] args)
        {
            try
            {
                _nc = CacheFactory.GetCache(_cacheName);
                _nc.ObjectExpired += HandleObjectExpired;
                _traceSource.TraceEvent(TraceEventType.Verbose, 1, $"Registered for event handling for named cache '{_cacheName}'");
            }
            catch (Exception ex)
            {
                _traceSource.TraceData(TraceEventType.Error, 2, ex);
                throw;
            }
        }

        /// <summary>
        /// Event handler for expiring objects in a ScaleOut named cache.
        /// </summary>
        private void HandleObjectExpired(object sender, NamedCacheObjExpiredEventArgs eventArgs)
        {
            try
            {
                // TODO: custom event handling logic:
                var expiringkey = eventArgs.CachedObjectId;
                object obj = _nc.Retrieve(expiringkey, acquireLock: false);
                _traceSource.TraceEvent(TraceEventType.Verbose, 101, obj.ToString());

                // Allow the expiring object to be removed from the store. (Or, set to 
                // NamedCacheObjDisposition.Save for objects that should not be removed.)
                eventArgs.NamedCacheObjDisposition = NamedCacheObjDisposition.Remove;

                // Trace successful completion.
                _traceSource.TraceEvent(TraceEventType.Verbose, 3, "Handled expiration event successfully.");
            }
            catch (Exception ex)
            {
                _traceSource.TraceEvent(TraceEventType.Warning, 4, $"Error handling expiration event:\n{ex}");
            }
        }


        protected override void OnStop()
        {
        }

        public void Debug(string[] args)
        {
            OnStart(args);
        }
    }
}
