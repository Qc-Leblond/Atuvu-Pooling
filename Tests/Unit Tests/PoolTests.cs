using System.Text.RegularExpressions;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Atuvu.Pooling.Tests
{
    sealed class PoolTests
    {
        sealed class TestComponent : MonoBehaviour, IPoolable
        {
            public int popCount { get; private set; }
            public int releaseCount { get; private set; }

            public void OnPop()
            {
                ++popCount;
            }

            public void OnRelease()
            {
                ++releaseCount;
            }
        }

        GameObject m_TestObject;
        Pool m_SizeOnePool;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            PoolManager.Initialize();
        }

        [SetUp]
        public void SetUp()
        {
            m_TestObject = new GameObject();
            m_TestObject.AddComponent<TestComponent>();

            m_SizeOnePool = Pool.CreatePool(m_TestObject, 1, ScaleResetMode.Disabled, OverflowMode.Expand);
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(m_TestObject);
            if (m_SizeOnePool != null) Object.Destroy(m_SizeOnePool);
        }

        [Test]
        public void CreatePool_RightValues()
        {

            var pool = Pool.CreatePool(m_TestObject, 5, ScaleResetMode.Disabled, OverflowMode.DontExpand);

            Assert.AreEqual(m_TestObject, pool.original);
            Assert.AreEqual(5, pool.capacity);
            Assert.AreEqual(ScaleResetMode.Disabled, pool.scaleResetMode);
            Assert.AreEqual(OverflowMode.DontExpand, pool.overflowMode);

            Object.Destroy(pool);
        }

        [Test]
        public void UninitializedPool_Initialize_PoolIsInitialized()
        {
            var pool = Pool.CreatePool(m_TestObject, 5, initialize: false);
            AssumeNotInitializeValid(pool);
            pool.Initialize();
            AssertInitializedProperly(pool);

            Object.Destroy(pool);
        }

        [Test]
        public void UninitializedPool_Pop_PoolIsInitialized()
        {
            var pool = Pool.CreatePool(m_TestObject, 5, initialize: false);

            AssumeNotInitializeValid(pool);
            pool.Pop();
            AssertInitializedProperly(pool);

            Object.Destroy(pool);
        }

        [Test]
        public void UninitializedPool_Release_PoolIsInitialized()
        {
            var pool = Pool.CreatePool(m_TestObject, 5, initialize: false);

            AssumeNotInitializeValid(pool);
            pool.Release(null); 
            AssertInitializedProperly(pool);

            Object.Destroy(pool);
        }

        void AssumeNotInitializeValid(Pool pool)
        {
            Assume.That(pool.capacity, Is.EqualTo(0));
        }

        void AssertInitializedProperly(Pool pool)
        {
            Assert.AreEqual(5, pool.capacity);
        }

        [Test]
        public void Pop_ElementsRemain_ObjectOfTheProperTypeReturned()
        {
            GameObject instance = m_SizeOnePool.Pop();

            Assert.AreEqual(0, m_SizeOnePool.availableCount);
            Assert.AreEqual(1, m_SizeOnePool.capacity);
            Assert.IsNotNull(instance.GetComponent<TestComponent>());
        }

        [Test]
        public void Pop_OverflowModeExpand_NoElementsRemain_ReturnNewElement()
        {
            m_SizeOnePool.Pop();
            Assume.That(m_SizeOnePool.availableCount, Is.EqualTo(0));

            GameObject instance = m_SizeOnePool.Pop();

            Assert.IsNotNull(instance);
            Assert.AreEqual(2, m_SizeOnePool.capacity);
            Assert.AreEqual(0, m_SizeOnePool.availableCount);
            Assert.IsNotNull(instance.GetComponent<TestComponent>());
        }

        [Test]
        public void Pop_OverflowModeDontExpand_NoElementsRemain_ReturnNull()
        {
            var pool = Pool.CreatePool(m_TestObject, 1, ScaleResetMode.Disabled, OverflowMode.DontExpand);

            pool.Pop();
            Assume.That(pool.availableCount, Is.EqualTo(0));

            GameObject instance = pool.Pop();

            Assert.IsNull(instance);
            Assert.AreEqual(1, pool.capacity);

            Object.Destroy(pool);
        }

        [Test]
        public void Pop_WithPositionRotationParent_HasProperTransform()
        {
            Transform parent = new GameObject().transform;

            GameObject instance = m_SizeOnePool.Pop(Vector3.one, Quaternion.Euler(10, 10, 10), parent);

            Assume.That(instance, Is.Not.Null);
            Assert.AreEqual(Vector3.one, instance.transform.position);
            Assert.AreEqual(Quaternion.Euler(10, 10,10), instance.transform.rotation);
            Assert.AreEqual(parent, instance.transform.parent);

            Object.Destroy(parent.gameObject);
        }

        [Test]
        public void Pop_ObjectIsActive()
        {
            GameObject go = new GameObject();
            go.SetActive(false);
            var pool = Pool.CreatePool(go, 1, ScaleResetMode.Disabled, OverflowMode.Expand);
            var obj = pool.Pop();

            Assert.AreEqual(true, obj.activeSelf);
        }

        [Test]
        public void Pop_WithComponentTemplate_ComponentExists_ReturnsComponent()
        {
            var component = m_SizeOnePool.Pop<TestComponent>();
            
            Assert.AreEqual(0, m_SizeOnePool.availableCount);
            Assert.IsNotNull(component);
        }

        [Test]
        public void Pop_WithComponentTemplate_ComponentDoesNotExists_ErrorObjectStaysInPool()
        {
            var component = m_SizeOnePool.Pop<CircleCollider2D>();
            
            LogAssert.Expect(LogType.Error, "Trying to Pop a pool object with a component of type CircleCollider2D but the component isn't present on the root object.");
            Assert.AreEqual(1, m_SizeOnePool.availableCount);
            Assert.IsNull(component);
        }

        [Test]
        public void Pop_ComponentReceivesCallback()
        {
            var component = m_SizeOnePool.Pop<TestComponent>();

            Assert.AreEqual(1, component.popCount);
        }

        [Test]
        public void Release_ObjectReturnsToPool()
        {
            GameObject obj = m_SizeOnePool.Pop();
            Assume.That(obj, Is.Not.Null);
            Assume.That(m_SizeOnePool.availableCount, Is.EqualTo(0));

            m_SizeOnePool.Release(obj);
            Assert.That(m_SizeOnePool.availableCount, Is.EqualTo(1));
        }

        [Test]
        public void Release_ObjectIsSetToPoolPosition()
        {
            GameObject obj = m_SizeOnePool.Pop();
            Assume.That(obj, Is.Not.Null);
            Assume.That(m_SizeOnePool.availableCount, Is.EqualTo(0));

            m_SizeOnePool.Release(obj);
            Assert.AreEqual(PoolManager.settings.poolsPosition, obj.transform.position);
        }

        [Test]
        [TestCase(false, Description = "disableObjectInPool = false", ExpectedResult = true)]
        [TestCase(true, Description = "disableObjectInPool = true", ExpectedResult = false)]
        public bool Release_ObjectIsDisabledDependingOnSettings(bool disableObjectInPool)
        {
            var obj = m_SizeOnePool.Pop();
            Assume.That(obj.activeSelf, Is.True);

            var settings = PoolManagerSettings.GetRAW();
            var prevValue = settings.disableObjectInPool;
            settings.disableObjectInPool = disableObjectInPool;
            
            m_SizeOnePool.Release(obj);

            settings.disableObjectInPool = prevValue;

            return obj.activeSelf;
        }

        [Test]
        [TestCase(ScaleResetMode.Disabled, 2f, 5f, ExpectedResult = 5)]
        [TestCase(ScaleResetMode.ResetToInitial, 2f, 5f, ExpectedResult = 2)]
        [TestCase(ScaleResetMode.ResetToOne, 2f, 5f, ExpectedResult = 1)]
        public int Release_ObjectHasItsScaleResetProperly(ScaleResetMode mode, float initScaleValue, float setScaleValue)
        {
            Vector3 initScale = Vector3.one * initScaleValue;
            m_TestObject.transform.localScale = initScale;

            var pool = Pool.CreatePool(m_TestObject, 1, mode, OverflowMode.Expand);
            var obj = pool.Pop();
            obj.transform.localScale = Vector3.one * setScaleValue;

            pool.Release(obj);
            Vector3 endScale = obj.transform.localScale;

            Object.Destroy(pool);

            return (int)endScale.x;
        }

        [Test]
        public void Release_ObjectNotCreatedByPool_NotAddedToPoolAndError()
        {
            GameObject wrongObject = new GameObject();

            m_SizeOnePool.Release(wrongObject);
            LogAssert.Expect(LogType.Error, new Regex("Trying to release"));

            Object.Destroy(wrongObject);
        }
        
        [Test]
        public void Release_ComponentReceivesCallback()
        {
            var component = m_SizeOnePool.Pop<TestComponent>();
            Assume.That(component.popCount, Is.EqualTo(1));

            m_SizeOnePool.Release(component.gameObject);

            Assume.That(component.popCount, Is.EqualTo(1));
        }

        [Test]
        [TestCase(10, 8, Description = "Ensure Less - Stay Same Capacity", ExpectedResult = 10)]
        [TestCase(10, 10, Description = "Ensure Same - Stay Same Capacity", ExpectedResult = 10)]
        [TestCase(10, 15, Description = "Ensure More - Resize to Ensure", ExpectedResult = 15)]
        public int EnsureCapacity_InitSize_SizeEnsured_SizeResult(int initSize, int sizeEnsured)
        {
            var pool = Pool.CreatePool(m_TestObject, initSize, ScaleResetMode.Disabled, OverflowMode.Expand);
            Assume.That(pool.capacity, Is.EqualTo(initSize));

            pool.EnsureCapacity(sizeEnsured);
            int capacity = pool.capacity;

            Object.Destroy(pool);

            return capacity;
        }

        public void DestroyPool_AllInUseAndAvailableObjectAreDestroyed()
        {
            var inUse = m_SizeOnePool.Pop();

            var insideObject = m_SizeOnePool.Pop();
            m_SizeOnePool.Release(insideObject);
            
            Object.Destroy(m_SizeOnePool);

            Assert.That(inUse == null);
            Assert.That(insideObject == null);
        }
    }
}