﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;
using Glimpse.Core;
using Glimpse.Core.Extensibility;
using Glimpse.Mvc.AlternateImplementation;
using Glimpse.Test.Common;
using Moq;
using Xunit;
using Xunit.Extensions;

namespace Glimpse.Test.Mvc3.AlternateImplementation
{
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Class is okay because it only changes the generic T parameter for the abstract class below.")]
    public class ValidatedValueProviderGetValueShould : ValueProviderGetValueShould<IValueProvider>
    {
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Class is okay because it only changes the generic T parameter for the abstract class below.")]
    public class UnvalidatedValueProviderGetValueShould : ValueProviderGetValueShould<IUnvalidatedValueProvider>
    {
    }

    public abstract class ValueProviderGetValueShould<T> where T : class
    {
        [Theory, AutoMock]
        public void ImplementProperMethod(ValueProvider<T>.GetValue<T> sut)
        {
            Assert.Equal("GetValue", sut.MethodToImplement.Name);
        }

        [Theory, AutoMock]
        public void ProceedAndAbortWithRuntimePolicyOff(ValueProvider<T>.GetValue<T> sut, IAlternateImplementationContext context)
        {
            context.Setup(c => c.RuntimePolicyStrategy).Returns(() => RuntimePolicy.Off);

            sut.NewImplementation(context);

            context.Verify(c => c.Proceed());
            context.MessageBroker.Verify(mb => mb.Publish(It.IsAny<object>()), Times.Never());
        }

        [Theory, AutoMock]
        public void ProceedAndPublishMessageWithRuntimePolicyOn(ValueProvider<T>.GetValue<T> sut, IAlternateImplementationContext context, string arg1, bool arg2)
        {
            context.Setup(c => c.Arguments).Returns(new object[] { arg1, arg2 });

            sut.NewImplementation(context);

            context.TimerStrategy().Verify(t => t.Time(It.IsAny<Action>()));
            context.MessageBroker.Verify(mb => mb.Publish(It.IsAny<ValueProvider<T>.GetValue<T>.Message>()));
        }
    }
}