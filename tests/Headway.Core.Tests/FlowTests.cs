﻿using Headway.Core.Enums;
using Headway.Core.Exceptions;
using Headway.Core.Extensions;
using Headway.Core.Model;
using Headway.Core.Tests.Helpers;
using Headway.SeedData.RemediatR;
using System.Text.Json;

namespace Headway.Core.Tests
{
    [TestClass]
    public class FlowTests
    {
        [TestMethod]
        public void RootState()
        {
            // Arrange
            var flow = RemediatRFlow.CreateRemediatRFlow();

            // Act
            var rootState = flow.RootState;

            // Assert
            Assert.AreEqual(flow.States.First(s => s.StateCode.Equals("REDRESS_CREATE")), rootState);
        }

        [TestMethod]
        [ExpectedException(typeof(FlowException))]
        public void Bootstrap_Already_Bootstrapped()
        {
            // Arrange
            var flow = FlowHelper.CreateFlow(2);

            flow.Bootstrapped = true;

            try
            {
                // Act
                flow.Bootstrap();
            }
            catch (FlowException ex)
            {
                // Assert
                Assert.AreEqual($"{flow.Name} already {nameof(flow.Bootstrapped)}.", ex.Message);

                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(FlowException))]
        public void Bootstrap_ConfigureFlowClass_Invalid()
        {
            // Arrange
            var flow = FlowHelper.CreateFlow(2);

            flow.FlowConfigurationClass = "Invalid ConfigureFlowClass";

            try
            {
                // Act
                flow.Bootstrap();
            }
            catch (FlowException ex)
            {
                // Assert
                Assert.AreEqual($"Can't resolve {flow.FlowConfigurationClass}", ex.Message);

                throw;
            }
        }

        [TestMethod]
        public void Bootstrap_Not_Started()
        {
            // Arrange
            var flow = RemediatRFlow.CreateRemediatRFlow();

            // Act
            flow.Bootstrap();

            // Assert
            Assert.AreEqual(FlowStatus.NotStarted, flow.FlowStatus);
            Assert.AreEqual(flow.States.FirstState(), flow.RootState);
            Assert.AreEqual(flow.States.FirstState(), flow.ActiveState);
            Assert.AreEqual(flow.States.LastState(), flow.FinalState);
        }

        [TestMethod]
        public void Flow_NotStarted()
        {
            // Arrange
            var flow = FlowHelper.CreateFlow(2);

            // Act
            flow.Bootstrap();

            //Assert
            Assert.AreEqual(FlowStatus.NotStarted, flow.FlowStatus);
        }

        [TestMethod]
        public async Task Flow_InProgress()
        {
            // Arrange
            var flow = FlowHelper.CreateFlow(2);

            flow.Bootstrap();

            // Act
            await flow.ActiveState.InitialiseAsync().ConfigureAwait(false);

            //Assert
            Assert.AreEqual(FlowStatus.InProgress, flow.FlowStatus);
        }

        [TestMethod]
        public async Task Flow_Completed()
        {
            // Arrange
            var flow = FlowHelper.CreateFlow(2);

            flow.States[0].TransitionStateCodes = $"{flow.States[1].StateCode}";

            flow.Bootstrap();

            await flow.ActiveState.InitialiseAsync().ConfigureAwait(false);

            await flow.ActiveState.CompleteAsync().ConfigureAwait(false);

            // Act
            await flow.ActiveState.CompleteAsync().ConfigureAwait(false);

            //Assert
            Assert.AreEqual(FlowStatus.Completed, flow.FlowStatus);
            Assert.AreEqual(flow.FinalState, flow.ActiveState);
        }

        [TestMethod]
        public async Task Flow_Completed_Regress_To_Prior_State_Flow_InProgress()
        {
            // Arrange
            var flow = FlowHelper.CreateFlow(3);

            flow.States[0].TransitionStateCodes = $"{flow.States[1].StateCode}";
            flow.States[1].TransitionStateCodes = $"{flow.States[2].StateCode}";
            flow.States[2].RegressionStateCodes = $"{flow.States[1].StateCode}";

            flow.Bootstrap();

            await flow.ActiveState.InitialiseAsync().ConfigureAwait(false);

            await flow.ActiveState.CompleteAsync().ConfigureAwait(false);

            await flow.ActiveState.CompleteAsync().ConfigureAwait(false);

            await flow.ActiveState.CompleteAsync().ConfigureAwait(false);

            Assert.AreEqual(FlowStatus.Completed, flow.FlowStatus);

            // Act
            await flow.FinalState.ResetAsync(flow.States[1].StateCode);

            //Assert
            Assert.AreEqual(FlowStatus.InProgress, flow.FlowStatus);
            Assert.AreEqual(flow.ActiveState, flow.States[1]);
        }

        [TestMethod]
        public async Task Flow_Completed_Regress_To_Root_State_Flow_NotStarted()
        {
            // Arrange
            var flow = FlowHelper.CreateFlow(3);

            flow.States[0].TransitionStateCodes = $"{flow.States[1].StateCode}";
            flow.States[1].TransitionStateCodes = $"{flow.States[2].StateCode}";
            flow.States[2].RegressionStateCodes = $"{flow.States[0].StateCode}";

            flow.Bootstrap();

            await flow.ActiveState.InitialiseAsync().ConfigureAwait(false);

            await flow.ActiveState.CompleteAsync().ConfigureAwait(false);

            await flow.ActiveState.CompleteAsync().ConfigureAwait(false);

            await flow.ActiveState.CompleteAsync().ConfigureAwait(false);

            Assert.AreEqual(FlowStatus.Completed, flow.FlowStatus);

            // Act
            await flow.FinalState.ResetAsync(flow.States[0].StateCode);

            //Assert
            Assert.AreEqual(FlowStatus.NotStarted, flow.FlowStatus);
            Assert.AreEqual(flow.RootState, flow.ActiveState);
        }

        //[TestMethod]
        //public async Task Flow_History_REDRESS_CREATE_UNINITIALIZED_TO_FINAL_REDRESS_REVIEW_COMPLETED()
        //{
        //    // Arrange
        //    var flow = RemediatRFlow.CreateRemediatRFlow();
        //    var json = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "RemediatRFlowHistory_FINAL_REDRESS_REVIEW.txt"));
        //    var expectedHistory = JsonSerializer.Deserialize<List<FlowHistory>>(json);

        //    // Act
        //    flow.Bootstrap();
        //    await flow.ActiveState.InitialiseAsync().ConfigureAwait(false);
        //    await flow.ActiveState.CompleteAsync().ConfigureAwait(false);
        //    await flow.ActiveState.CompleteAsync().ConfigureAwait(false);
        //    await flow.ActiveState.CompleteAsync().ConfigureAwait(false);
        //    await flow.ActiveState.CompleteAsync().ConfigureAwait(false);
        //    await flow.ActiveState.CompleteAsync().ConfigureAwait(false);
        //    await flow.ActiveState.CompleteAsync().ConfigureAwait(false);
        //    await flow.ActiveState.CompleteAsync().ConfigureAwait(false);
        //    await flow.ActiveState.CompleteAsync().ConfigureAwait(false);
        //    await flow.ActiveState.CompleteAsync().ConfigureAwait(false);
        //    await flow.ActiveState.CompleteAsync().ConfigureAwait(false);

        //    // Assert
        //    Assert.AreEqual(expectedHistory.Count, flow.History.Count);

        //    for (int i = 0; i < flow.History.Count; i++)
        //    {
        //        Assert.AreEqual(expectedHistory[i].Event, flow.History[i].Event);
        //        Assert.AreEqual(expectedHistory[i].StateCode, flow.History[i].StateCode);
        //        Assert.AreEqual(expectedHistory[i].StateStatus, flow.History[i].StateStatus);
        //    }
        //}

        //[TestMethod]
        //public async Task Flow_History_REDRESS_REVIEW_INITIALIZED_TO_FINAL_REDRESS_REVIEW_COMPLETED()
        //{
        //    // Arrange
        //    var flow = RemediatRFlow.CreateRemediatRFlow();
        //    var json = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "RemediatRFlowHistory_FINAL_REDRESS_REVIEW.txt"));
        //    var expectedHistory = JsonSerializer.Deserialize<List<FlowHistory>>(json);
        //    var redressReview = expectedHistory.Take(11).ToList();

        //    // Act
        //    flow.Bootstrap(redressReview);
        //    await flow.ActiveState.CompleteAsync();
        //    await flow.ActiveState.CompleteAsync();
        //    await flow.ActiveState.CompleteAsync();
        //    await flow.ActiveState.CompleteAsync();
        //    await flow.ActiveState.CompleteAsync();
        //    await flow.ActiveState.CompleteAsync();

        //    // Assert
        //    Assert.AreEqual(expectedHistory.Count, flow.History.Count);

        //    for (int i = 0; i < flow.History.Count; i++)
        //    {
        //        Assert.AreEqual(expectedHistory[i].Event, flow.History[i].Event);
        //        Assert.AreEqual(expectedHistory[i].FlowCode, flow.History[i].FlowCode);
        //        Assert.AreEqual(expectedHistory[i].StateCode, flow.History[i].StateCode);
        //        Assert.AreEqual(expectedHistory[i].StateStatus, flow.History[i].StateStatus);
        //    }
        //}
    }
}
