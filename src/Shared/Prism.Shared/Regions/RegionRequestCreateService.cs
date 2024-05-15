using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Common;
using Prism.Ioc;
using Prism.Properties;
using Prism.Ioc.Internals;
using System.Drawing;
using System.Globalization;

namespace Prism.Regions
{
    public class RegionRequestCreateService : IRegionRequestCreateService
    {
        private readonly IContainerExtension _container;

        /// <summary>
        /// Gets or sets the region.
        /// </summary>
        /// <value>The region.</value>
        public IRegion Region { get; set; }

        public RegionRequestCreateService(IContainerExtension container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public RequestCreateResult RequestCreate(Uri target, bool cancelIfExists)
        {
            return RequestCreate(target, null, cancelIfExists);
        }

        public RequestCreateResult RequestCreate(Uri target, NavigationParameters parameters, bool cancelIfExists)
        {
            RequestCreateContext context = new RequestCreateContext(this, target, parameters);
            try
            {
                string candidateTargetContract = UriParsingHelper.GetContract(target);
                if (cancelIfExists)
                {
                    var candidates = GetCandidatesFromRegion(this.Region, candidateTargetContract);
                    if (candidates.Any())
                    {
                        throw new RequestCreateCanceledException();
                    }
                }
                var view = CreateNewRegionItem(candidateTargetContract);
                context.AssociatedView = new WeakReference(view);

                NotifyNewRegionItemCreated(view, context);

                AddViewToRegion(this.Region, view);

                return new RequestCreateResult(context, true);
            }
            catch (Exception ex)
            {
                return new RequestCreateResult(context, ex);
            }
        }

        private void NotifyNewRegionItemCreated(object view, RequestCreateContext context)
        {
            Action<IRequestCreateAware> action =
                (n) => n.OnCreated(context);
            MvvmHelpers.ViewAndViewModelAction(view, action);
        }

        /// <summary>
        /// Adds the view to the region.
        /// </summary>
        /// <param name="region">The region to add the view to</param>
        /// <param name="view">The view to add to the region</param>
        protected virtual void AddViewToRegion(IRegion region, object view)
        {
            region.Add(view);
        }

        /// <summary>
        /// Provides a new item for the region based on the supplied candidate target contract name.
        /// </summary>
        /// <param name="candidateTargetContract">The target contract to build.</param>
        /// <returns>An instance of an item to put into the <see cref="IRegion"/>.</returns>
        protected virtual object CreateNewRegionItem(string candidateTargetContract)
        {
            try
            {
                var newRegionItem = _container.Resolve<object>(candidateTargetContract);
                MvvmHelpers.AutowireViewModel(newRegionItem);
                return newRegionItem;
            }
            catch (ContainerResolutionException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(
                    string.Format(CultureInfo.CurrentCulture, Resources.CannotCreateNavigationTarget, candidateTargetContract),
                    e);
            }
        }

        /// <summary>
        /// Returns the set of candidates that may satisfy this request create.
        /// </summary>
        /// <param name="region">The region containing items that may satisfy the request create.</param>
        /// <param name="candidateNavigationContract">The candidate request target as determined by <see cref="GetContractFromNavigationContext"/></param>
        /// <returns>An enumerable of candidate objects from the <see cref="IRegion"/></returns>
        protected virtual IEnumerable<object> GetCandidatesFromRegion(IRegion region, string candidateNavigationContract)
        {
            return RegionHelper.GetNavigationCandidates(region, candidateNavigationContract);
        }
    }
}
