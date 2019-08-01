using UnityEngine;
using NUnit.Framework;

namespace Atuvu.Pooling.Tests
{
    sealed class PoolTests
    {
        [Test]
        public void UninitializedPool_Initialize_PoolIsInitialized()
        {
            AssertInitializedProperly();
        }

        [Test]
        public void UninitializedPool_Pop_PoolIsInitialized()
        {
            AssertInitializedProperly();
        }

        [Test]
        public void UninitializedPool_Release_PoolIsInitialized()
        {
            AssertInitializedProperly();
        }

        void AssertInitializedProperly()
        {
        }

        [Test]
        public void Pop_ElementsRemain_ObjectOfTheProperTypeReturned()
        {
        }

        [Test]
        public void Pop_OverflowModeExpand_NoElementsRemain_ReturnNewElement()
        {
            //TODO asserts capacity
        }

        [Test]
        public void Pop_OverflowModeDontExpand_NoElementsRemain_ReturnNull()
        {
            //TODO asserts capacity
        }

        [Test]
        public void Pop_WithPositionRotationParent_HasProperTransform()
        {

        }

        [Test]
        public void Pop_ObjectIsActive()
        {

        }

        [Test]
        public void Pop_AvailableObjectMinusOne()
        {
        }

        [Test]
        public void Release_ObjectReturnsToPool()
        {
        }

        [Test]
        public void Release_ObjectIsSetToPoolPosition()
        {
        }

        [Test]
        public void Release_ObjectIsDisabledDependingOnSettings()
        {
            //Set fake setting to right value
        }

        [Test]
        [TestCase(ScaleResetMode.Disabled, 2f, 5f, ExpectedResult = 5)]
        [TestCase(ScaleResetMode.ResetToInitial, 2f, 5f, ExpectedResult = 2)]
        [TestCase(ScaleResetMode.Disabled, 2f, 5f, ExpectedResult = 1)]
        public int Release_ObjectHasItsScaleResetProperly(ScaleResetMode mode, float initScaleValue, float setScaleValue)
        {
            Vector3 initScale = Vector3.one * initScaleValue;

            //TODO get from pool
            //TODO set gameObject scale to target scale

            return (int)0f; //TODO Send scale result
        }

        [Test]
        public void Release_ObjectNotCreatedByPool_NotAddedToPoolAndError()
        {
        }

        [Test]
        public void Release_AvailableObjectPlusOne()
        {
        }

        [Test]
        [TestCase(10, 8, Description = "Ensure Less - Stay Same Capacity", ExpectedResult = 10)]
        [TestCase(10, 10, Description = "Ensure Same - Stay Same Capacity", ExpectedResult = 10)]
        [TestCase(10, 15, Description = "Ensure More - Resize to Ensure", ExpectedResult = 15)]
        public int EnsureCapacity_InitSize_SizeEnsured_SizeResult(int initSize, int sizeEnsured)
        {
            return 0;
        }
    }
}