using NUnit.Framework;
using DAL.Repo.Roles;
using Core.Entities.Roles;
using System.Threading.Tasks;



namespace DAL.NUnitTests1
{
    [TestFixture]
    public class clsRolesRepoTests
    {
        private clsRolesRepo _rolesRepo;

        [SetUp]
        public void Setup()
        {
            _rolesRepo = new clsRolesRepo();
        }

        #region 1. اختبارات القراءة والبحث

        [Test]
        public async Task GetAllRolesAsync_ShouldReturnListOfRoles()
        {
            // Act
            var roles = await _rolesRepo.GetAllRolesAsync();

            // Assert
            Assert.IsNotNull(roles, "فشل: القائمة المرجعة null!");
            Assert.IsTrue(roles.Count > 0, "فشل: لا توجد أدوار في قاعدة البيانات.");
        }

        [TestCase(1)]
        public async Task GetRoleByIdAsync_WithValidId_ShouldReturnRole(long roleId)
        {
            // Act
            var role = await _rolesRepo.GetRoleByIdAsync(roleId);

            // Assert
            Assert.IsNotNull(role, $"فشل: لم يتم العثور على الدور رقم {roleId}");
            Assert.AreEqual(roleId, role.ROL_ID, "فشل: المعرف المرجع لا يطابق المطلوب.");
        }

        [TestCase(999999)]
        public async Task GetRoleByIdAsync_WithInvalidId_ShouldReturnNull(long invalidId)
        {
            // Act
            var role = await _rolesRepo.GetRoleByIdAsync(invalidId);

            // Assert
            Assert.IsNull(role, "فشل: كان المتوقع إرجاع null لمعرف غير موجود.");
        }

        [Test]
        public async Task GetRolesPagedAsync_ShouldReturnPagedList()
        {
            // Act: جلب الصفحة الأولى وبها 5 عناصر
            var roles = await _rolesRepo.GetRolesPagedAsync(pageNumber: 1, rowsPerPage: 5, searchQuery: null);

            // Assert
            Assert.IsNotNull(roles, "فشل: نتيجة التصفح أرجعت null!");
        }

        [Test]
        public async Task GetTotalRolesCountAsync_ShouldReturnCount()
        {
            // Act
            var count = await _rolesRepo.GetTotalRolesCountAsync(searchQuery: null);

            // Assert
            Assert.IsTrue(count >= 0, "فشل: إجمالي العدد يجب أن يكون 0 أو أكثر.");
        }

        [Test]
        public async Task GetMaxRoleIdAsync_ShouldReturnMaxId()
        {
            // Act
            var maxId = await _rolesRepo.GetMaxRoleIdAsync();

            // Assert
            Assert.IsTrue(maxId >= 0, "فشل: أعلى ID يجب أن يكون 0 أو أكثر.");
        }

        #endregion

        #region 2. اختبار الإضافة والتعديل والحذف (Integration Test Sequence)

        [Test]
        public async Task Role_CRUD_Operations_ShouldWorkSuccessfully()
        {
            var newRole = new clsRole
            {
                ROL_KEY = "TEST_ROLE_" + System.DateTime.Now.Ticks,
                ROL_NAME = "دور تجريبي",
                ROL_DESCRIPTION = "وصف اختبار NUnit",
                STATUS = true
            };

            long newId = await _rolesRepo.AddRoleAsync(newRole);
            Assert.IsTrue(newId > 0, "فشل: لم يتم إضافة الدور الجديد بنجاح!");

            newRole.ROL_ID = newId;
            newRole.ROL_NAME = "دور تجريبي معدل";
            bool isUpdated = await _rolesRepo.UpdateRoleAsync(newRole);
            Assert.IsTrue(isUpdated, "فشل: لم يتم تعديل الدور!");

            bool isDeleted = await _rolesRepo.DeleteRoleAsync(newId);
            Assert.IsTrue(isDeleted, "فشل: لم يتم حذف الدور التجريبي!");
        }

        #endregion
    }
}
