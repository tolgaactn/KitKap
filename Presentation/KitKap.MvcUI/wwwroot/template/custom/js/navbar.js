// ✅ KURUMSAL STANDART: Profil Dropdown (Sadece Click)
document.addEventListener('DOMContentLoaded', function () {
    const profileDropdown = document.getElementById('profileDropdown');
    const profileToggle = document.getElementById('profileToggle');
    const profileMenu = document.getElementById('profileMenu');

    if (!profileDropdown || !profileToggle || !profileMenu) {
        console.warn('Profil dropdown elementleri bulunamadı');
        return;
    }

    // Dropdown toggle (sadece tıklama)
    profileToggle.addEventListener('click', function (e) {
        e.preventDefault();
        e.stopPropagation();

        const isOpen = profileDropdown.classList.contains('show');

        // Diğer tüm dropdownları kapat
        document.querySelectorAll('.header-dropdown.show').forEach(dropdown => {
            if (dropdown !== profileDropdown) {
                dropdown.classList.remove('show');
            }
        });

        // Bu dropdown'u toggle et
        profileDropdown.classList.toggle('show');
        profileToggle.setAttribute('aria-expanded', !isOpen);
    });

    // Dropdown dışına tıklanınca kapat
    document.addEventListener('click', function (e) {
        if (!profileDropdown.contains(e.target)) {
            profileDropdown.classList.remove('show');
            profileToggle.setAttribute('aria-expanded', 'false');
        }
    });

    // Dropdown içindeki linklere tıklanınca kapat (form submit hariç)
    profileMenu.querySelectorAll('.dropdown-item:not(button)').forEach(item => {
        item.addEventListener('click', function () {
            profileDropdown.classList.remove('show');
            profileToggle.setAttribute('aria-expanded', 'false');
        });
    });

    // ESC tuşu ile kapat
    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape' && profileDropdown.classList.contains('show')) {
            profileDropdown.classList.remove('show');
            profileToggle.setAttribute('aria-expanded', 'false');
        }
    });
});