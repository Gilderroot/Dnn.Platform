﻿#region Copyright
// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2014
// by DotNetNuke Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Content.DynamicContent;
using DotNetNuke.Services.Cache;
using DotNetNuke.Tests.Utilities;
using DotNetNuke.Tests.Utilities.Mocks;
using Moq;
using NUnit.Framework;

namespace DotNetNuke.Tests.Content.DynamicContent
{
    [TestFixture]
    public class ValidationRuleControllerTests
    {
        private Mock<IDataContext> _mockDataContext;
        private Mock<IRepository<ValidationRule>> _mockValidationRuleRepository;
        // ReSharper disable once NotAccessedField.Local
        private Mock<CachingProvider> _mockCache;

        [SetUp]
        public void SetUp()
        {
            //Register MockCachingProvider
            _mockCache = MockComponentProvider.CreateNew<CachingProvider>();
            MockComponentProvider.CreateDataProvider().Setup(c => c.GetProviderPath()).Returns(String.Empty);

            _mockDataContext = new Mock<IDataContext>();

            _mockValidationRuleRepository = new Mock<IRepository<ValidationRule>>();
            _mockDataContext.Setup(dc => dc.GetRepository<ValidationRule>()).Returns(_mockValidationRuleRepository.Object);
        }

        [TearDown]
        public void TearDown()
        {
            MockComponentProvider.ResetContainer();
        }

        [Test]
        public void AddValidationRule_Throws_On_Null_ValidationRule()
        {
            //Arrange
            var validationRuleController = new ValidationRuleController(_mockDataContext.Object);

            //Act, Arrange
            Assert.Throws<ArgumentNullException>(() => validationRuleController.AddValidationRule(null));
        }

        [Test]
        public void AddValidationRule_Throws_On_Negative_FieldDefinitionId_Property()
        {
            //Arrange
            var validationRuleController = new ValidationRuleController(_mockDataContext.Object);

            var validationRule = new ValidationRule
                                    {
                                        FieldDefinitionId = Constants.CONTENTTYPE_InValidFieldDefinitionId,
                                        ValidatorTypeId = Constants.CONTENTTYPE_ValidValidatorTypeId
                                    };

            //Act, Arrange
            Assert.Throws<ArgumentOutOfRangeException>(() => validationRuleController.AddValidationRule(validationRule));
        }

        [Test]
        public void AddValidationRule_Throws_On_Negative_ValidatorTypeId_Property()
        {
            //Arrange
            var validationRuleController = new ValidationRuleController(_mockDataContext.Object);

            var validationRule = new ValidationRule
                                    {
                                        FieldDefinitionId = Constants.CONTENTTYPE_InValidFieldDefinitionId,
                                        ValidatorTypeId = Constants.CONTENTTYPE_InValidValidatorTypeId
                                    };

            //Act, Arrange
            Assert.Throws<ArgumentOutOfRangeException>(() => validationRuleController.AddValidationRule(validationRule));
        }

        [Test]
        public void AddValidationRule_Calls_Repository_Insert_On_Valid_Arguments()
        {
            //Arrange
            var validationRuleController = new ValidationRuleController(_mockDataContext.Object);

            var validationRule = new ValidationRule
                                {
                                    FieldDefinitionId = Constants.CONTENTTYPE_ValidFieldDefinitionId,
                                    ValidatorTypeId = Constants.CONTENTTYPE_ValidValidatorTypeId
                                };

            //Act
            // ReSharper disable once UnusedVariable
            int validationRuleId = validationRuleController.AddValidationRule(validationRule);

            //Assert
            _mockValidationRuleRepository.Verify(rep => rep.Insert(validationRule));
        }

        [Test]
        public void AddValidationRule_Returns_ValidId_On_Valid_ValidationRule()
        {
            //Arrange
            var mockValidationRuleController = new Mock<IValidationRuleController>();
            mockValidationRuleController.Setup(vr => vr.GetValidationSettings(It.IsAny<int>()))
                                    .Returns(new Dictionary<string, ValidatorSetting>());
            ValidationRuleController.SetTestableInstance(mockValidationRuleController.Object);

            _mockValidationRuleRepository.Setup(r => r.Insert(It.IsAny<ValidationRule>()))
                            .Callback((ValidationRule df) => df.ValidationRuleId = Constants.CONTENTTYPE_AddValidationRuleId);

            var validationRuleController = new ValidationRuleController(_mockDataContext.Object);

            var validationRule = new ValidationRule
                                    {
                                        FieldDefinitionId = Constants.CONTENTTYPE_ValidFieldDefinitionId,
                                        ValidatorTypeId = Constants.CONTENTTYPE_ValidValidatorTypeId
                                    };

            //Act
            int validationRuleId = validationRuleController.AddValidationRule(validationRule);

            //Assert
            Assert.AreEqual(Constants.CONTENTTYPE_AddValidationRuleId, validationRuleId);
        }

        [Test]
        public void AddValidationRule_Sets_ValidId_On_Valid_ValidationRule()
        {
            //Arrange
            var mockValidationRuleController = new Mock<IValidationRuleController>();
            mockValidationRuleController.Setup(vr => vr.GetValidationSettings(It.IsAny<int>()))
                                    .Returns(new Dictionary<string, ValidatorSetting>());
            ValidationRuleController.SetTestableInstance(mockValidationRuleController.Object);

            _mockValidationRuleRepository.Setup(r => r.Insert(It.IsAny<ValidationRule>()))
                            .Callback((ValidationRule dt) => dt.ValidationRuleId = Constants.CONTENTTYPE_AddValidationRuleId);

            var validationRuleController = new ValidationRuleController(_mockDataContext.Object);

            var validationRule = new ValidationRule
                                    {
                                        FieldDefinitionId = Constants.CONTENTTYPE_ValidFieldDefinitionId,
                                        ValidatorTypeId = Constants.CONTENTTYPE_ValidValidatorTypeId
                                    };

            //Act
            validationRuleController.AddValidationRule(validationRule);

            //Assert
            Assert.AreEqual(Constants.CONTENTTYPE_AddValidationRuleId, validationRule.ValidationRuleId);
        }

        [Test]
        public void AddValidationRule_Calls_Repository_Insert_For_ValidationSettings()
        {
            //Arrange
            var validationRuleId = Constants.CONTENTTYPE_AddValidationRuleId;
            _mockValidationRuleRepository.Setup(r => r.Insert(It.IsAny<ValidationRule>()))
                            .Callback((ValidationRule dt) => dt.ValidationRuleId = validationRuleId);
            var mockValidatorSettingRepository = new Mock<IRepository<ValidatorSetting>>();
            _mockDataContext.Setup(dc => dc.GetRepository<ValidatorSetting>()).Returns(mockValidatorSettingRepository.Object);


            var validationRuleController = new ValidationRuleController(_mockDataContext.Object);

            var validationRule = new ValidationRule
                                        {
                                            FieldDefinitionId = Constants.CONTENTTYPE_ValidFieldDefinitionId,
                                            ValidatorTypeId = Constants.CONTENTTYPE_ValidValidatorTypeId
                                        };
            var validatorSetting = new ValidatorSetting
                                        {
                                            SettingName = "Name",
                                            SettingValue = "Value"
                                        };

            validationRule.ValidationSettings.Add(validatorSetting.SettingName, validatorSetting);

            //Act
            validationRuleController.AddValidationRule(validationRule);

            //Assert
            mockValidatorSettingRepository.Verify(settingRep => settingRep.Insert(validatorSetting));
        }

        [Test]
        public void AddValidationRule_Sets_ValidationRuleId_Property_Of_ValidationSettings()
        {
            //Arrange
            var validationRuleId = Constants.CONTENTTYPE_AddValidationRuleId;
            _mockValidationRuleRepository.Setup(r => r.Insert(It.IsAny<ValidationRule>()))
                            .Callback((ValidationRule dt) => dt.ValidationRuleId = validationRuleId);
            var mockValidatorSettingRepository = new Mock<IRepository<ValidatorSetting>>();
            _mockDataContext.Setup(dc => dc.GetRepository<ValidatorSetting>()).Returns(mockValidatorSettingRepository.Object);


            var validationRuleController = new ValidationRuleController(_mockDataContext.Object);

            var validationRule = new ValidationRule
                                        {
                                            FieldDefinitionId = Constants.CONTENTTYPE_ValidFieldDefinitionId,
                                            ValidatorTypeId = Constants.CONTENTTYPE_ValidValidatorTypeId
                                        };
            var validatorSetting = new ValidatorSetting
                                        {
                                            SettingName = "Name",
                                            SettingValue = "Value"
                                        };

            validationRule.ValidationSettings.Add(validatorSetting.SettingName, validatorSetting);

            //Act
            validationRuleController.AddValidationRule(validationRule);

            //Assert
            Assert.AreEqual(validationRuleId, validatorSetting.ValidationRuleId);
        }

        [Test]
        public void DeleteValidationRule_Throws_On_Null_ValidationRule()
        {
            //Arrange
            var validationRuleController = new ValidationRuleController(_mockDataContext.Object);

            //Act, Arrange
            Assert.Throws<ArgumentNullException>(() => validationRuleController.DeleteValidationRule(null));
        }

        [Test]
        public void DeleteValidationRule_Throws_On_Negative_ValidationRuleId()
        {
            //Arrange
            var validationRuleController = new ValidationRuleController(_mockDataContext.Object);

            var validationRule = new ValidationRule { ValidationRuleId = Null.NullInteger };

            //Act, Arrange
            Assert.Throws<ArgumentOutOfRangeException>(() => validationRuleController.DeleteValidationRule(validationRule));
        }

        [Test]
        public void DeleteValidationRule_Calls_Repository_Delete_On_Valid_ValidationRuleId()
        {
            //Arrange
            var mockValidatorSettingRepository = new Mock<IRepository<ValidatorSetting>>();
            _mockDataContext.Setup(dc => dc.GetRepository<ValidatorSetting>()).Returns(mockValidatorSettingRepository.Object);

            var validationRuleController = new ValidationRuleController(_mockDataContext.Object);

            var validationRule = new ValidationRule
            {
                ValidationRuleId = Constants.CONTENTTYPE_ValidValidationRuleId
            };

            //Act
            validationRuleController.DeleteValidationRule(validationRule);

            //Assert
            _mockValidationRuleRepository.Verify(r => r.Delete(validationRule));
        }

        [Test]
        public void DeleteValidationRule_Calls_Repository_Delete_For_ValidationSettings()
        {
            //Arrange
            var validationRuleId = Constants.CONTENTTYPE_ValidValidationRuleId;
            _mockValidationRuleRepository.Setup(r => r.Insert(It.IsAny<ValidationRule>()))
                            .Callback((ValidationRule dt) => dt.ValidationRuleId = validationRuleId);
            var mockValidatorSettingRepository = new Mock<IRepository<ValidatorSetting>>();
            _mockDataContext.Setup(dc => dc.GetRepository<ValidatorSetting>()).Returns(mockValidatorSettingRepository.Object);


            var validationRuleController = new ValidationRuleController(_mockDataContext.Object);

            var validationRule = new ValidationRule
                                        {
                                            ValidationRuleId = validationRuleId,
                                            FieldDefinitionId = Constants.CONTENTTYPE_ValidFieldDefinitionId,
                                            ValidatorTypeId = Constants.CONTENTTYPE_ValidValidatorTypeId
                                        };
            var validatorSetting = new ValidatorSetting
                                        {
                                            SettingName = "Name",
                                            SettingValue = "Value"
                                        };

            validationRule.ValidationSettings.Add(validatorSetting.SettingName, validatorSetting);

            //Act
            validationRuleController.DeleteValidationRule(validationRule);

            //Assert
            mockValidatorSettingRepository.Verify(settingRep => settingRep.Delete(validatorSetting));
        }

        [Test]
        public void GetValidationRules_Calls_Repository_Get()
        {
            //Arrange
            var validationRuleController = new ValidationRuleController(_mockDataContext.Object);

            //Act
            // ReSharper disable once UnusedVariable
            var validationRules = validationRuleController.GetValidationRules(Constants.CONTENTTYPE_ValidFieldDefinitionId);

            //Assert
            _mockValidationRuleRepository.Verify(r => r.Get(Constants.CONTENTTYPE_ValidFieldDefinitionId));
        }

        [Test]
        public void GetValidationRules_Returns_Empty_List_Of_ValidationRules_If_No_ValidationRules()
        {
            //Arrange
            _mockValidationRuleRepository.Setup(r => r.Get(Constants.CONTENTTYPE_ValidFieldDefinitionId))
                .Returns(new List<ValidationRule>());
            var validationRuleController = new ValidationRuleController(_mockDataContext.Object);

            //Act
            var validationRules = validationRuleController.GetValidationRules(Constants.CONTENTTYPE_ValidFieldDefinitionId);

            //Assert
            Assert.IsNotNull(validationRules);
            Assert.AreEqual(0, validationRules.Count());
        }

        [Test]
        public void GetValidationRules_Returns_List_Of_ValidationRules()
        {
            //Arrange
            _mockValidationRuleRepository.Setup(r => r.Get(Constants.CONTENTTYPE_ValidFieldDefinitionId))
                .Returns(GetValidValidationRules(Constants.CONTENTTYPE_ValidValidationRuleCount));
            var validationRuleController = new ValidationRuleController(_mockDataContext.Object);

            //Act
            var validationRules = validationRuleController.GetValidationRules(Constants.CONTENTTYPE_ValidFieldDefinitionId);

            //Assert
            Assert.AreEqual(Constants.CONTENTTYPE_ValidValidationRuleCount, validationRules.Count());
        }

        [Test]
        public void GetValidationSettings_Calls_Settings_Repository_Get()
        {
            //Arrange
            var validationRuleId = Constants.CONTENTTYPE_ValidValidationRuleId;
            var validationRuleController = new ValidationRuleController(_mockDataContext.Object);
            var mockValidatorSettingRepository = new Mock<IRepository<ValidatorSetting>>();
            _mockDataContext.Setup(dc => dc.GetRepository<ValidatorSetting>()).Returns(mockValidatorSettingRepository.Object);

            //Act
            // ReSharper disable once UnusedVariable
            var validationRules = validationRuleController.GetValidationSettings(validationRuleId);

            //Assert
            mockValidatorSettingRepository.Verify(r => r.Get(validationRuleId));
        }

        [Test]
        public void GetValidationSettings_Returns_Empty_List_Of_ValidationSettings_If_No_ValidationSettings()
        {
            //Arrange
            var validationRuleId = Constants.CONTENTTYPE_ValidValidationRuleId;
            var mockValidatorSettingRepository = new Mock<IRepository<ValidatorSetting>>();
            _mockDataContext.Setup(dc => dc.GetRepository<ValidatorSetting>()).Returns(mockValidatorSettingRepository.Object);
            mockValidatorSettingRepository.Setup(r => r.Get(validationRuleId))
                .Returns(new List<ValidatorSetting>());
            var validationRuleController = new ValidationRuleController(_mockDataContext.Object);

            //Act
            var settings = validationRuleController.GetValidationSettings(validationRuleId);

            //Assert
            Assert.IsNotNull(settings);
            Assert.AreEqual(0, settings.Count());
        }

        [Test]
        public void GetValidationSettings_Returns_List_Of_ValidationSettings()
        {
            //Arrange
            var settingCount = 5;
            var validationRuleId = Constants.CONTENTTYPE_ValidValidationRuleId;
            var mockValidatorSettingRepository = new Mock<IRepository<ValidatorSetting>>();
            _mockDataContext.Setup(dc => dc.GetRepository<ValidatorSetting>()).Returns(mockValidatorSettingRepository.Object);
            mockValidatorSettingRepository.Setup(r => r.Get(validationRuleId))
                .Returns(GetValidatorSettings(settingCount, validationRuleId));
            var validationRuleController = new ValidationRuleController(_mockDataContext.Object);

            //Act
            var settings = validationRuleController.GetValidationSettings(validationRuleId);

            //Assert
            Assert.AreEqual(settingCount, settings.Count());
        }

        [Test]
        public void UpdateValidationRule_Throws_On_Null_ValidationRule()
        {
            //Arrange
            var validationRuleController = new ValidationRuleController(_mockDataContext.Object);

            //Act, Arrange
            Assert.Throws<ArgumentNullException>(() => validationRuleController.UpdateValidationRule(null));
        }

        [Test]
        public void UpdateValidationRule_Throws_On_Negative_FieldDefinitionId_Property()
        {
            //Arrange
            var validationRuleController = new ValidationRuleController(_mockDataContext.Object);

            var validationRule = new ValidationRule
                                    {
                                        ValidationRuleId = Constants.CONTENTTYPE_ValidValidationRuleId,
                                        FieldDefinitionId = Constants.CONTENTTYPE_InValidFieldDefinitionId,
                                        ValidatorTypeId = Constants.CONTENTTYPE_ValidValidatorTypeId
                                    };

            //Act, Arrange
            Assert.Throws<ArgumentOutOfRangeException>(() => validationRuleController.UpdateValidationRule(validationRule));
        }

        [Test]
        public void UpdateValidationRule_Throws_On_Negative_ValidatorTypeId_Property()
        {
            //Arrange
            var validationRuleController = new ValidationRuleController(_mockDataContext.Object);

            var validationRule = new ValidationRule
                                    {
                                        FieldDefinitionId = Constants.CONTENTTYPE_ValidFieldDefinitionId,
                                        ValidationRuleId = Constants.CONTENTTYPE_ValidValidationRuleId,
                                        ValidatorTypeId = Constants.CONTENTTYPE_InValidValidatorTypeId
                                    };

            //Act, Arrange
            Assert.Throws<ArgumentOutOfRangeException>(() => validationRuleController.UpdateValidationRule(validationRule));
        }

        [Test]
        public void UpdateValidationRule_Throws_On_Negative_ValidationRuleId()
        {
            //Arrange
            var validationRuleController = new ValidationRuleController(_mockDataContext.Object);

            var field = new ValidationRule
                            {
                                FieldDefinitionId = Constants.CONTENTTYPE_InValidFieldDefinitionId,
                                ValidationRuleId = Constants.CONTENTTYPE_ValidValidationRuleId,
                                ValidatorTypeId = Constants.CONTENTTYPE_ValidValidatorTypeId
                            };

            Assert.Throws<ArgumentOutOfRangeException>(() => validationRuleController.UpdateValidationRule(field));
        }

        [Test]
        public void UpdateValidationRule_Calls_Repository_Update()
        {
            //Arrange
            var mockValidationRuleController = new Mock<IValidationRuleController>();
            mockValidationRuleController.Setup(vr => vr.GetValidationSettings(It.IsAny<int>()))
                                    .Returns(new Dictionary<string, ValidatorSetting>());
            ValidationRuleController.SetTestableInstance(mockValidationRuleController.Object);

            var validationRuleController = new ValidationRuleController(_mockDataContext.Object);

            var field = new ValidationRule
                            {
                                FieldDefinitionId = Constants.CONTENTTYPE_ValidFieldDefinitionId,
                                ValidationRuleId = Constants.CONTENTTYPE_ValidValidationRuleId,
                                ValidatorTypeId = Constants.CONTENTTYPE_ValidValidatorTypeId
                            };

            //Act
            validationRuleController.UpdateValidationRule(field);

            //Assert
            _mockValidationRuleRepository.Verify(r => r.Update(field));
        }

        [Test]
        public void UpdateValidationRule_Calls_Repository_Insert_For_New_ValidationSettings()
        {
            //Arrange
            var mockValidationRuleController = new Mock<IValidationRuleController>();
            mockValidationRuleController.Setup(vr => vr.GetValidationSettings(It.IsAny<int>()))
                                    .Returns(new Dictionary<string, ValidatorSetting>());
            ValidationRuleController.SetTestableInstance(mockValidationRuleController.Object);

            var validationRuleId = Constants.CONTENTTYPE_ValidValidationRuleId;
            _mockValidationRuleRepository.Setup(r => r.Insert(It.IsAny<ValidationRule>()))
                            .Callback((ValidationRule dt) => dt.ValidationRuleId = validationRuleId);
            var mockValidatorSettingRepository = new Mock<IRepository<ValidatorSetting>>();
            _mockDataContext.Setup(dc => dc.GetRepository<ValidatorSetting>()).Returns(mockValidatorSettingRepository.Object);


            var validationRuleController = new ValidationRuleController(_mockDataContext.Object);

            var validationRule = new ValidationRule
                                        {
                                            ValidationRuleId = validationRuleId,
                                            FieldDefinitionId = Constants.CONTENTTYPE_ValidFieldDefinitionId,
                                            ValidatorTypeId = Constants.CONTENTTYPE_ValidValidatorTypeId
                                        };
            var validatorSetting = new ValidatorSetting
                                        {
                                            SettingName = "Name",
                                            SettingValue = "Value"
                                        };

            validationRule.ValidationSettings.Add(validatorSetting.SettingName, validatorSetting);

            //Act
            validationRuleController.UpdateValidationRule(validationRule);

            //Assert
            mockValidatorSettingRepository.Verify(settingRep => settingRep.Insert(validatorSetting));
        }

        [Test]
        public void UpdateValidationRule_Calls_Repository_Update_For_Existing_ValidationSettings()
        {
            //Arrange
            var validationRuleId = Constants.CONTENTTYPE_ValidValidationRuleId;
            _mockValidationRuleRepository.Setup(r => r.Insert(It.IsAny<ValidationRule>()))
                            .Callback((ValidationRule dt) => dt.ValidationRuleId = validationRuleId);
            var mockValidatorSettingRepository = new Mock<IRepository<ValidatorSetting>>();
            _mockDataContext.Setup(dc => dc.GetRepository<ValidatorSetting>()).Returns(mockValidatorSettingRepository.Object);


            var validationRuleController = new ValidationRuleController(_mockDataContext.Object);

            var validationRule = new ValidationRule
                                        {
                                            ValidationRuleId = validationRuleId,
                                            FieldDefinitionId = Constants.CONTENTTYPE_ValidFieldDefinitionId,
                                            ValidatorTypeId = Constants.CONTENTTYPE_ValidValidatorTypeId
                                        };
            var validatorSetting = new ValidatorSetting
                                        {
                                            ValidatorSettingId = 2,
                                            SettingName = "Name",
                                            SettingValue = "Value"
                                        };

            validationRule.ValidationSettings.Add(validatorSetting.SettingName, validatorSetting);

            //Act
            validationRuleController.UpdateValidationRule(validationRule);

            //Assert
            mockValidatorSettingRepository.Verify(settingRep => settingRep.Update(validatorSetting));
        }

        [Test]
        public void UpdateValidationRule_Sets_ValidationRuleId_Property_Of_ValidationSettings()
        {
            //Arrange
            var validationRuleId = Constants.CONTENTTYPE_AddValidationRuleId;
            _mockValidationRuleRepository.Setup(r => r.Insert(It.IsAny<ValidationRule>()))
                            .Callback((ValidationRule dt) => dt.ValidationRuleId = validationRuleId);
            var mockValidatorSettingRepository = new Mock<IRepository<ValidatorSetting>>();
            _mockDataContext.Setup(dc => dc.GetRepository<ValidatorSetting>()).Returns(mockValidatorSettingRepository.Object);


            var validationRuleController = new ValidationRuleController(_mockDataContext.Object);

            var validationRule = new ValidationRule
            {
                FieldDefinitionId = Constants.CONTENTTYPE_ValidFieldDefinitionId,
                ValidatorTypeId = Constants.CONTENTTYPE_ValidValidatorTypeId
            };
            var validatorSetting = new ValidatorSetting
            {
                SettingName = "Name",
                SettingValue = "Value"
            };

            validationRule.ValidationSettings.Add(validatorSetting.SettingName, validatorSetting);

            //Act
            validationRuleController.AddValidationRule(validationRule);

            //Assert
            Assert.AreEqual(validationRuleId, validatorSetting.ValidationRuleId);
        }

        private List<ValidatorSetting> GetValidatorSettings(int count, int validationRuleId)
        {
            var list = new List<ValidatorSetting>();

            for (int i = 1; i <= count; i++)
            {
                list.Add(new ValidatorSetting()
                                {
                                    ValidationRuleId = validationRuleId,
                                    SettingName = String.Format("Name_{0}", i),
                                    SettingValue = String.Format("Value_{0}", i)
                });
            }

            return list;
        }

        private List<ValidationRule> GetValidValidationRules(int count)
        {
            var list = new List<ValidationRule>();

            for (int i = 1; i <= count; i++)
            {
                list.Add(new ValidationRule() { ValidationRuleId = i, ValidatorTypeId = i, FieldDefinitionId = i });
            }

            return list;
        }

    }
}
