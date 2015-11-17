using System;
using System.Collections.ObjectModel;
using System.Linq;
using OpenQA.Selenium;

namespace Selenium.Extensions
{
    public class TestSearchContext: ISearchContext
    {
        private readonly ISearchContext _context;
        private readonly Func<IWebElement> _selfLookup;
        private readonly By _selfSelector;
        private readonly TestSettings _settings;

        public TestSearchContext(TestSettings settings, By selfSelector, ISearchContext context,Func<IWebElement> selfLookup)
        {
            _settings = settings;
            _selfSelector = selfSelector;
            _context = context;
            _selfLookup = selfLookup;
        }

        public IWebElement FindElement(By @by)
        {
            var element = _context as IWebElement;
            if (element != null)
            {
                //Console.WriteLine("element->element: {0}", @by);
                return TestInteractionWrapper.Interact(
                    ref element,
                    _selfSelector,
                    () => _selfLookup(),
                    lmnt => new TestWebElement(_settings, lmnt, _selfSelector, this, ctx => ctx.FindElement(@by))
                    );
            }

            //context is IWebDriver, no need to guard for stale element
            //Console.WriteLine("driver->element: {0}", @by);
            element = _context.FindElement(@by);
            return new TestWebElement(_settings, element, @by, _context, ctx => ctx.FindElement(@by));
        }

        public ReadOnlyCollection<IWebElement> FindElements(By @by)
        {
            var element = _context as IWebElement;

            return new EagerReadOnlyCollection<IWebElement>(() =>
                (element != null
                    ? TestInteractionWrapper.Interact(ref element, _selfSelector, () => _selfLookup(),
                        lmnt => _selfLookup().FindElements(@by))
                    : _context.FindElements(@by))
                    .Select((lmnt, index) =>
                    {
                        //each element must be able to re-resolve it self
                        //in this case, re-resolve all elements again and just pick the
                        //element that has the same index as before
                        return new TestWebElement(_settings, lmnt, @by, this,
                            ctx =>
                            {
                                var children = _selfLookup == null
                                    ? _context.FindElements(@by)
                                    : _selfLookup().FindElements(@by);

                                return children.ElementAt(index);
                            });
                    })
                );
        }
    }
}