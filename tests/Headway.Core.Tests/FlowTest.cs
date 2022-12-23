﻿using Headway.Core.Enums;
using Headway.Core.Extensions;
using Headway.Core.Model;
using Headway.Core.Tests.Helpers;
using Headway.SeedData.RemediatR;

namespace Headway.Core.Tests
{
    [TestClass]
    public class FlowTest
    {
        [TestMethod]
        public void StateExtensions_FirstState()
        {
            // Arrange
            var states = new List<State>
            {
                new State { Position = 100 },
                new State { Position = 50 },
                new State { Position = 10 },
                new State { Position = 250 }
            };

            // Act
            var firstState = states.FirstState();

            // Assert
            Assert.AreEqual(firstState, states.Single(s => s.Position == 10));
        }

        [TestMethod]
        public void Flow_RootState()
        {
            // Arrange
            var flow = RemediatRFlow.CreateRemediatRFlow();

            // Act
            var rootState = flow.RootState;

            // Assert
            Assert.AreEqual(flow.States.First(s => s.StateCode.Equals("REDRESS_CREATE")), rootState);
        }

        [TestMethod]
        public void Flow_Bootstrap_No_History()
        {
            // Arrange
            var flow = RemediatRFlow.CreateRemediatRFlow();

            // Act
            flow.Bootstrap();

            // Assert active state
            Assert.AreEqual(flow.RootState, flow.States.FirstState());
            Assert.AreEqual(flow.ActiveState, flow.States.FirstState());

            // Assert sample of parent and substates
            Assert.AreEqual(flow.States.First(s => s.StateCode.Equals("REFUND_ASSESSMENT")), flow.States.First(s => s.StateCode.Equals("REFUND_CALCULATION")).ParentState);
            Assert.AreEqual(flow.States.First(s => s.StateCode.Equals("REFUND_ASSESSMENT")), flow.States.First(s => s.StateCode.Equals("REFUND_VERIFICATION")).ParentState);
            Assert.IsTrue(flow.States.First(s => s.StateCode.Equals("REFUND_ASSESSMENT")).SubStates.Contains(flow.States.First(s => s.StateCode.Equals("REFUND_CALCULATION"))));
            Assert.IsTrue(flow.States.First(s => s.StateCode.Equals("REFUND_ASSESSMENT")).SubStates.Contains(flow.States.First(s => s.StateCode.Equals("REFUND_VERIFICATION"))));

            // Assert sample of transition states
            Assert.IsTrue(flow.States.First(s => s.StateCode.Equals("REDRESS_CREATE")).Transitions.Contains(flow.States.First(s => s.StateCode.Equals("REFUND_ASSESSMENT"))));
            Assert.IsTrue(flow.States.First(s => s.StateCode.Equals("REFUND_ASSESSMENT")).Transitions.Contains(flow.States.First(s => s.StateCode.Equals("REDRESS_CREATE"))));
            Assert.IsTrue(flow.States.First(s => s.StateCode.Equals("REFUND_ASSESSMENT")).Transitions.Contains(flow.States.First(s => s.StateCode.Equals("REFUND_REVIEW"))));
        }

        [TestMethod]
        public void Flow_Bootstrap_With_History()
        {
            // Arrange

            // Act

            // Assert
        }

        [TestMethod]
        public async Task State_Initialise_ActiveState_No_SubStates()
        {
            // Arrange
            var flow = RemediatRFlow.CreateRemediatRFlow();

            flow.Bootstrap();

            // Act
            await flow.ActiveState.InitialiseAsync().ConfigureAwait(false);

            // Assert
            Assert.AreEqual(flow.ActiveState.StateStatus, StateStatus.InProgress);
        }

        [TestMethod]
        public async Task State_Initialise_State_Has_SubStates()
        {
            // Arrange
            var flow = RemediatRFlow.CreateRemediatRFlow();

            flow.Bootstrap();

            // Act
            await flow.States.Find(s => s.StateCode.Equals("REFUND_ASSESSMENT")).InitialiseAsync().ConfigureAwait(false);

            // Assert
            Assert.AreEqual(flow.States.First(s => s.StateCode.Equals("REFUND_ASSESSMENT")).StateStatus, StateStatus.InProgress);
            Assert.AreEqual(flow.States.First(s => s.StateCode.Equals("REFUND_CALCULATION")).StateStatus, StateStatus.InProgress);
            Assert.AreEqual(flow.States.First(s => s.StateCode.Equals("REFUND_CALCULATION")), flow.ActiveState);
            Assert.AreEqual(flow.States.First(s => s.StateCode.Equals("REFUND_VERIFICATION")).StateStatus, StateStatus.NotStarted);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task State_Initialise_State_Already_InProgress()
        {
            // Arrange
            var flow = FlowTestHelper.CreateFlow(2);

            flow.States[0].TransitionStateCodes = $"{flow.States[1].StateCode}";

            flow.ActionSetupClass = "Headway.Core.Tests.Helpers.FlowStateActionHelper, Headway.Core.Tests";

            flow.Bootstrap();

            flow.States[1].StateStatus = StateStatus.InProgress;

            try
            {
                // Act
                await flow.States[1].InitialiseAsync().ConfigureAwait(false);
            }
            catch (InvalidOperationException ex)
            {
                // Assert
                Assert.AreEqual(ex.Message, $"Can't initialize {flow.States[1].StateStatus} because it's already {StateStatus.InProgress}.");

                throw;
            }
        }

        [TestMethod]
        public async Task State_Initialise_State_Has_FlowStateActions()
        {
            // Arrange
            var flow = RemediatRFlow.CreateRemediatRFlow();
            
            flow.ActionSetupClass = "Headway.Core.Tests.Helpers.FlowStateActionHelper, Headway.Core.Tests";

            flow.Bootstrap();

            // Act
            await flow.ActiveState.InitialiseAsync().ConfigureAwait(false);

            // Assert
            Assert.AreEqual(flow.ActiveState, flow.States.FirstState());
            Assert.AreEqual(flow.ActiveState.StateStatus, StateStatus.InProgress);
            Assert.AreEqual(flow.ActiveState.Context, $"1 Initialize {flow.ActiveState.StateCode}; 2 Initialize {flow.ActiveState.StateCode}");
        }

        [TestMethod]
        public async Task State_Initialise_State_Has_StateActions()
        {
            // Arrange
            var flow = RemediatRFlow.CreateRemediatRFlow();

            flow.Bootstrap();

            flow.ActiveState.ActionSetupClass = "Headway.Core.Tests.Helpers.StateActionHelper, Headway.Core.Tests";

            // Act
            await flow.ActiveState.InitialiseAsync().ConfigureAwait(false);

            // Assert
            Assert.AreEqual(flow.ActiveState, flow.States.FirstState());
            Assert.AreEqual(flow.ActiveState.StateStatus, StateStatus.InProgress);
            Assert.AreEqual(flow.ActiveState.Context, $"3 Initialize {flow.ActiveState.StateCode}; 4 Initialize {flow.ActiveState.StateCode}");
        }

        [TestMethod]
        public async Task State_Initialise_State_Has_FlowStateActions_And_StateActions()
        {
            // Arrange
            var flow = RemediatRFlow.CreateRemediatRFlow();

            flow.ActionSetupClass = "Headway.Core.Tests.Helpers.FlowStateActionHelper, Headway.Core.Tests";

            flow.Bootstrap();

            flow.ActiveState.ActionSetupClass = "Headway.Core.Tests.Helpers.StateActionHelper, Headway.Core.Tests";

            var activeStateContext = $"1 Initialize {flow.ActiveState.StateCode}; 2 Initialize {flow.ActiveState.StateCode}; 3 Initialize {flow.ActiveState.StateCode}; 4 Initialize {flow.ActiveState.StateCode}";

            // Act
            await flow.ActiveState.InitialiseAsync().ConfigureAwait(false);

            // Assert
            Assert.AreEqual(flow.ActiveState, flow.States.FirstState());
            Assert.AreEqual(flow.ActiveState.StateStatus, StateStatus.InProgress);
            Assert.AreEqual(flow.ActiveState.Context, activeStateContext);
        }

        [TestMethod]
        public async Task State_Transition_Default()
        {
            // Arrange
            var flow = FlowTestHelper.CreateFlow(2);

            flow.States[0].TransitionStateCodes = $"{flow.States[1].StateCode}";

            flow.Bootstrap();

            // Act
            await flow.ActiveState.CompleteAsync().ConfigureAwait(false);

            // Assert
            Assert.AreEqual(flow.States[0].StateStatus, StateStatus.Completed);
            Assert.AreEqual(flow.States[1].StateStatus, StateStatus.InProgress);
            Assert.AreEqual(flow.States[1], flow.ActiveState);
        }

        [TestMethod]
        public async Task State_Transition_To_StateCode()
        {
            // Arrange
            var flow = FlowTestHelper.CreateFlow(2);

            flow.States[0].TransitionStateCodes = $"{flow.States[1].StateCode}";

            flow.Bootstrap();

            // Act
            await flow.ActiveState.CompleteAsync(flow.States[1].StateCode).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(flow.States[0].StateStatus, StateStatus.Completed);
            Assert.AreEqual(flow.States[1].StateStatus, StateStatus.InProgress);
            Assert.AreEqual(flow.States[1], flow.ActiveState);
        }

        [TestMethod]
        public async Task State_Transition_To_ParentState()
        {
            // Arrange
            var flow = RemediatRFlow.CreateRemediatRFlow();

            flow.Bootstrap();

            // Act
            await flow.ActiveState.CompleteAsync("REFUND_ASSESSMENT").ConfigureAwait(false);

            // Assert
            Assert.AreEqual(flow.States.FirstState().StateStatus, StateStatus.Completed);
            Assert.AreEqual(flow.States.First(s => s.StateCode.Equals("REFUND_ASSESSMENT")).StateStatus, StateStatus.InProgress);
            Assert.AreEqual(flow.States.First(s => s.StateCode.Equals("REFUND_CALCULATION")).StateStatus, StateStatus.InProgress);
            Assert.AreEqual(flow.States.First(s => s.StateCode.Equals("REFUND_CALCULATION")), flow.ActiveState);
            Assert.AreEqual(flow.States.First(s => s.StateCode.Equals("REFUND_VERIFICATION")).StateStatus, StateStatus.NotStarted);
        }
    }
}
