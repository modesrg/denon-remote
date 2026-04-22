window.theme = {
    get: () => localStorage.getItem('theme') ?? 'modern',
    set: (name) => {
        localStorage.setItem('theme', name);
        document.documentElement.dataset.theme = name;
    }
};
