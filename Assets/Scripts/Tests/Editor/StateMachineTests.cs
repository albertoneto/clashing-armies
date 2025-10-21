#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace ClashingArmies.Tests
{
    public class StateMachineTests
    {
        private GameObject stateMachineObject;
        private StateMachine stateMachine;

        [SetUp]
        public void SetUp()
        {
            stateMachineObject = new GameObject("StateMachine");
            stateMachine = stateMachineObject.AddComponent<StateMachine>();
        }

        [TearDown]
        public void TearDown()
        {
            if (stateMachineObject != null)
                Object.DestroyImmediate(stateMachineObject);
        }

        [Test]
        public void AddState_ShouldAddStateSuccessfully()
        {
            var mockState = new MockState();

            Assert.DoesNotThrow(() => stateMachine.AddState(mockState));
        }

        [Test]
        public void AddState_DuplicateState_ShouldLogWarning()
        {
            var mockState1 = new MockState();
            var mockState2 = new MockState();

            stateMachine.AddState(mockState1);
            
            LogAssert.Expect(LogType.Warning, "State already exists: MockState");
            stateMachine.AddState(mockState2);
        }

        [Test]
        public void SetState_ShouldChangeCurrentState()
        {
            var mockState = new MockState();
            stateMachine.AddState(mockState);

            stateMachine.SetState<MockState>();

            Assert.AreEqual(mockState, stateMachine.CurrentState);
        }

        [Test]
        public void SetState_ShouldCallOnEnter()
        {
            var mockState = new MockState();
            stateMachine.AddState(mockState);

            stateMachine.SetState<MockState>();

            Assert.IsTrue(mockState.OnEnterCalled, "OnEnter should be called");
        }

        [Test]
        public void SetState_ShouldCallPreviousStateOnExit()
        {
            var state1 = new MockState();
            var state2 = new AnotherMockState();
            
            stateMachine.AddState(state1);
            stateMachine.AddState(state2);

            stateMachine.SetState<MockState>();
            stateMachine.SetState<AnotherMockState>();

            Assert.IsTrue(state1.OnExitCalled, "Previous state's OnExit should be called");
        }

        [Test]
        public void SetState_ToSameState_ShouldLogAndNotChange()
        {
            var mockState = new MockState();
            stateMachine.AddState(mockState);
            stateMachine.SetState<MockState>();

            mockState.OnEnterCalled = false;
            mockState.OnExitCalled = false;

            LogAssert.Expect(LogType.Log, "Already in state: MockState");
            stateMachine.SetState<MockState>();

            Assert.IsFalse(mockState.OnEnterCalled, "OnEnter should not be called again");
            Assert.IsFalse(mockState.OnExitCalled, "OnExit should not be called");
        }

        [Test]
        public void SetState_NonExistentState_ShouldLogError()
        {
            LogAssert.Expect(LogType.Error, "State not found: MockState. Did you forget to add it?");
            stateMachine.SetState<MockState>();
        }

        [Test]
        public void Update_ShouldCallCurrentStateOnUpdate()
        {
            var mockState = new MockState();
            stateMachine.AddState(mockState);
            stateMachine.SetState<MockState>();

            stateMachine.SendMessage("Update");

            Assert.IsTrue(mockState.OnUpdateCalled, "OnUpdate should be called");
        }

        [Test]
        public void FixedUpdate_ShouldCallCurrentStateOnFixedUpdate()
        {
            var mockState = new MockState();
            stateMachine.AddState(mockState);
            stateMachine.SetState<MockState>();

            stateMachine.SendMessage("FixedUpdate");

            Assert.IsTrue(mockState.OnFixedUpdateCalled, "OnFixedUpdate should be called");
        }

        [Test]
        public void Update_WithNoCurrentState_ShouldNotThrow()
        {
            Assert.DoesNotThrow(() => stateMachine.SendMessage("Update"));
        }

        [Test]
        public void OnDestroy_ShouldCallCurrentStateOnExit()
        {
            var mockState = new MockState();
            stateMachine.AddState(mockState);
            stateMachine.SetState<MockState>();

            Object.DestroyImmediate(stateMachineObject);

            Assert.IsTrue(mockState.OnExitCalled, "OnExit should be called on destroy");
        }

        [Test]
        public void StateTransition_ShouldMaintainCorrectOrder()
        {
            var state1 = new MockState();
            var state2 = new AnotherMockState();
            
            stateMachine.AddState(state1);
            stateMachine.AddState(state2);

            stateMachine.SetState<MockState>();
            Assert.IsTrue(state1.OnEnterCalled);
            Assert.IsFalse(state1.OnExitCalled);

            stateMachine.SetState<AnotherMockState>();
            Assert.IsTrue(state1.OnExitCalled, "state1 should exit");
            Assert.IsTrue(state2.OnEnterCalled, "state2 should enter");
        }

        [Test]
        public void CurrentState_InitiallyNull()
        {
            Assert.IsNull(stateMachine.CurrentState);
        }

        [Test]
        public void MultipleStateTransitions_ShouldWorkCorrectly()
        {
            var state1 = new MockState();
            var state2 = new AnotherMockState();
            var state3 = new ThirdMockState();
            
            stateMachine.AddState(state1);
            stateMachine.AddState(state2);
            stateMachine.AddState(state3);

            stateMachine.SetState<MockState>();
            Assert.AreEqual(state1, stateMachine.CurrentState);

            stateMachine.SetState<AnotherMockState>();
            Assert.AreEqual(state2, stateMachine.CurrentState);

            stateMachine.SetState<ThirdMockState>();
            Assert.AreEqual(state3, stateMachine.CurrentState);
        }

        private class MockState : IState
        {
            public bool OnEnterCalled { get; set; }
            public bool OnExitCalled { get; set; }
            public bool OnUpdateCalled { get; set; }
            public bool OnFixedUpdateCalled { get; set; }

            public void OnEnter() => OnEnterCalled = true;
            public void OnExit() => OnExitCalled = true;
            public void OnUpdate() => OnUpdateCalled = true;
            public void OnFixedUpdate() => OnFixedUpdateCalled = true;
        }

        private class AnotherMockState : IState
        {
            public bool OnEnterCalled { get; set; }
            public bool OnExitCalled { get; set; }

            public void OnEnter() => OnEnterCalled = true;
            public void OnExit() => OnExitCalled = true;
            public void OnUpdate() { }
            public void OnFixedUpdate() { }
        }

        private class ThirdMockState : IState
        {
            public void OnEnter() { }
            public void OnExit() { }
            public void OnUpdate() { }
            public void OnFixedUpdate() { }
        }
    }
}
#endif