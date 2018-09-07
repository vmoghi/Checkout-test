using System;
using Checkout_LivingLens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Moq;
namespace UnitTestCheckout
{
    [TestClass]
    public class UnitTest1
    {
        private FakeDatabase _fakeDatabase = null;
        private Checkout _checkout = null;
        Mock<IFakeDatabase> _mockFakeDB = null;
        [TestInitialize]
        public void SetUp()
        {
            _fakeDatabase = new FakeDatabase();
            _checkout = new Checkout(null, new List<ItemModel>());
            _mockFakeDB = new Mock<IFakeDatabase>();
        }
        [TestMethod]
        public void FakeDatabase_GetItemModel_Return_Null_Model_With_Empty_Parameter()
        {
            var itemModel = _fakeDatabase.GetItemModel("");
            Assert.IsNull(itemModel);
        }

        [TestMethod]
        public void FakeDatabase_GetItemModel_Return_Valid_Item_Model_From_Fake_Database()
        {
            var itemModel = _fakeDatabase.GetItemModel("bananas");
            Assert.IsInstanceOfType(itemModel, typeof(ItemModel));

        }

        [TestMethod]
        public void FakeDatabase_GetItemModel_Return_Item_Apple_Model_From_Fake_Database()
        {
            var itemModel = _fakeDatabase.GetItemModel("apple");

            Assert.AreEqual("apple", itemModel.Name.ToLower());
            Assert.AreEqual(0.50, itemModel.OriginalPrice);
            Assert.AreEqual(1, itemModel.Quantity);
            Assert.IsNull(itemModel.DiscountRule);
        }
        [TestMethod]
        public void FakeDatabase_GetItemModel_Return_Item_Orange_Model_From_Fake_Database_With_DiscountRule_Not_Null()
        {
            var itemModel = _fakeDatabase.GetItemModel("orange");
            var discountRule = itemModel.DiscountRule;
            Assert.IsNotNull(itemModel.DiscountRule);
            Assert.AreEqual(3, discountRule.Quantity);
            Assert.AreEqual(0.90, discountRule.DiscountedPrice);
        }

        [TestMethod]
        public void Checkout_Scan_Will_Not_Add_A_New_item_Model_In_Item_List_With_Empty_Parameter()
        {
            _checkout.Scan("");
            PrivateObject privateFakeDatabase = new PrivateObject(_checkout);
            var listItems = (IList<ItemModel>)privateFakeDatabase.GetField("_listItems");
            var listItemModels = listItems;
            Assert.AreEqual(0, listItemModels.Count);
        }
       
        [TestMethod]
        public void Checkout_Scan_Will_Add_A_New_item_Model_In_Item_List_From_Fake_Database()
        {
            ItemModel modelOrange = new ItemModel();
            modelOrange.Name = "orange";

            _mockFakeDB.Setup(fake => fake.GetItemModel(It.IsAny<string>())).Returns(modelOrange);
            var checkout = new Checkout(_mockFakeDB.Object, new List<ItemModel>());
            checkout.Scan("orange");

            PrivateObject privateCheckOut = new PrivateObject(checkout);
            var listItems = (IList<ItemModel>)privateCheckOut.GetField("_listItems");
            var listItemModels = listItems;
            Assert.AreEqual(1, listItemModels.Count);
        }
        [TestMethod]
        public void Checkout_Scan_Check_Same_Values_In_item_Model_And_Item_List_Stored()
        {
            var listItemsModels = new ItemModel();
            listItemsModels.Quantity = 1;
            listItemsModels.OriginalPrice = 0.70;
            listItemsModels.Name = "bananas";
            listItemsModels.DiscountRule = new DiscountRuleModel();
            listItemsModels.DiscountRule.Quantity = 2;
            listItemsModels.DiscountRule.DiscountedPrice = 1;

            _mockFakeDB.Setup(fake => fake.GetItemModel(It.IsAny<string>())).Returns(listItemsModels);

            var checkout = new Checkout(_mockFakeDB.Object, new List<ItemModel>());
            checkout.Scan("bananas");

            PrivateObject privateFakeDatabase = new PrivateObject(checkout);
            var listItems = (IList<ItemModel>)privateFakeDatabase.GetField("_listItems");
            var listItemModels = listItems;
            Assert.AreEqual("bananas", listItemModels[0].Name);
            Assert.AreEqual(1, listItemModels[0].Quantity);
            Assert.AreEqual(0.70, listItemModels[0].OriginalPrice);
            Assert.AreEqual(2, listItemModels[0].DiscountRule.Quantity);
            Assert.AreEqual(1, listItemModels[0].DiscountRule.DiscountedPrice);
        }


        [TestMethod]
        public void Checkout_Scan_Add_Two_item_Models_With_The_Different_Names_In_Item_List_Stored()
        {

            ItemModel modelOrange = new ItemModel();
            modelOrange.Name = "orange";
            ItemModel modelApple = new ItemModel();
            modelApple.Name = "apple";

            _mockFakeDB.Setup(fake => fake.GetItemModel(It.IsAny<string>())).Returns(modelOrange);
            var checkout = new Checkout(_mockFakeDB.Object, new List<ItemModel>());
            checkout.Scan("orange");

            PrivateObject privateFakeDatabase = new PrivateObject(checkout);
            privateFakeDatabase.SetField("_fakeDatabase", _mockFakeDB.Object);

            _mockFakeDB.Setup(fake => fake.GetItemModel(It.IsAny<string>())).Returns(modelApple);
            checkout.Scan("apple");

            var listItems = (IList<ItemModel>)privateFakeDatabase.GetField("_listItems");
            var listItemModels = listItems;
            Assert.AreEqual(2, listItemModels.Count);
            Assert.AreEqual("orange", listItemModels[0].Name);
            Assert.AreEqual("apple", listItemModels[1].Name);
        }

        [TestMethod]
        public void Checkout_Scan_Add_Only_One_item_Model_With_The_Same_Name_In_Item_List_Stored()
        {
            var modelApple = new ItemModel();
            modelApple.Name = "apple";
            modelApple.Quantity = 1;
            _mockFakeDB.Setup(fake => fake.GetItemModel(It.IsAny<string>())).Returns(modelApple);
            var checkout = new Checkout(_mockFakeDB.Object, new List<ItemModel>());
            checkout.Scan("apple");
            checkout.Scan("apple");

            PrivateObject privateFakeDatabase = new PrivateObject(checkout);
            var listItems = (IList<ItemModel>)privateFakeDatabase.GetField("_listItems");
            var listItemModels = listItems;
            Assert.AreEqual(1, listItemModels.Count);
            Assert.AreEqual(2, listItemModels[0].Quantity);
        }
        
        [TestMethod]
        public void Checkout_GetTotal_Will_Return_Zero_When_Empty_List()
        {
            var total = _checkout.Total();
            Assert.AreEqual(0, total);
        }
        
        [TestMethod]
        public void Checkout_GetTotal_Will_Return_Total_From_Item_Model_With_One_Quantity()
        {
            var listItems = new List<ItemModel>();
            var listItemsModels = new ItemModel();
            listItemsModels.Name = "apple";
            listItemsModels.Quantity = 1;
            listItemsModels.OriginalPrice = 0.5;
            listItems.Add(listItemsModels);

            var checkout = new Checkout(null, listItems);
            var total = checkout.Total();

            Assert.AreEqual(0.5, total);
        }
        
        [TestMethod]
        public void Checkout_GetTotal_Will_Return_Total_From_Item_Model_With_More_Quantity()
        {
            var listItems = new List<ItemModel>();
            var listItemsModels = new ItemModel();
            listItemsModels.Name = "apple";
            listItemsModels.Quantity = 3;
            listItemsModels.OriginalPrice = 0.5;
            listItems.Add(listItemsModels);

            var checkout = new Checkout(null, listItems);
            var total = checkout.Total();

            Assert.AreEqual(1.5, total);
        }
       
        [TestMethod]
        public void Checkout_GetTotal_Will_Return_Total_From_Different_Item_Models()
        {
            var listItems = new List<ItemModel>();
            var listItemsModelA = new ItemModel();
            listItemsModelA.Name = "apple";
            listItemsModelA.Quantity = 2;
            listItemsModelA.OriginalPrice = 0.5;
            listItems.Add(listItemsModelA);
            var listItemsModelB = new ItemModel();
            listItemsModelB.Name = "orange";
            listItemsModelB.Quantity = 1;
            listItemsModelB.OriginalPrice = 0.45;
            listItems.Add(listItemsModelB);

            var checkout = new Checkout(null, listItems);
            var total = checkout.Total();

            Assert.AreEqual(1.45, total);
        }
        
        [TestMethod]
        public void Checkout_GetTotal_Will_Return_Total_With_Discount_Rule()
        {
            var listItems = new List<ItemModel>();
            var listItemsModelA = new ItemModel();
            listItemsModelA.Name = "orange";
            listItemsModelA.Quantity = 3;
            listItemsModelA.OriginalPrice = 0.45;
            listItemsModelA.DiscountRule = new DiscountRuleModel();
            listItemsModelA.DiscountRule.Quantity = 3;
            listItemsModelA.DiscountRule.DiscountedPrice = 0.90;
            listItems.Add(listItemsModelA);

            var checkout = new Checkout(null, listItems);
            var total = checkout.Total();

            Assert.AreEqual(0.90, total);
        }
        
        [TestMethod]
        public void Checkout_GetTotal_Will_Return_Total_With_Discount_Rule_And_Full_Price()
        {
            var listItems = new List<ItemModel>();
            var listItemsModelA = new ItemModel();
            listItemsModelA.Name = "bananas";
            listItemsModelA.Quantity = 4;
            listItemsModelA.OriginalPrice = 0.50;
            listItemsModelA.DiscountRule = new DiscountRuleModel();
            listItemsModelA.DiscountRule.Quantity = 2;
            listItemsModelA.DiscountRule.DiscountedPrice = 1;
            listItems.Add(listItemsModelA);

            var checkout = new Checkout(null, listItems);
            var total = checkout.Total();

            Assert.AreEqual(2, total);
        }
        
        [TestMethod]
        public void Checkout_GetTotal_Will_Return_Total_With_Discount_Rule_And_Full_Price_With_Some_Items()
        {
            var listItems = new List<ItemModel>();
            var listItemsModelA = new ItemModel();
            listItemsModelA.Name = "orange";
            listItemsModelA.Quantity = 5;
            listItemsModelA.OriginalPrice = 0.45;
            listItemsModelA.DiscountRule = new DiscountRuleModel();
            listItemsModelA.DiscountRule.Quantity = 3;
            listItemsModelA.DiscountRule.DiscountedPrice = 0.90;
            listItems.Add(listItemsModelA);
            var listItemsModelB = new ItemModel();
            listItemsModelB.Name = "bananas";
            listItemsModelB.Quantity = 2;
            listItemsModelB.OriginalPrice = 0.7;
            listItemsModelB.DiscountRule = new DiscountRuleModel();
            listItemsModelB.DiscountRule.Quantity = 2;
            listItemsModelB.DiscountRule.DiscountedPrice = 1;
            listItems.Add(listItemsModelB);
            var listItemsModelC = new ItemModel();
            listItemsModelC.Name = "apple";
            listItemsModelC.Quantity = 3;
            listItemsModelC.OriginalPrice = 0.50;

            listItems.Add(listItemsModelC);

            var checkout = new Checkout(null, listItems);
            var total = checkout.Total();

            Assert.AreEqual(4.30, total);
        }
                
    }
}
