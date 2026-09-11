(() => {
    const formatDigits = value => {
        const digits = String(value ?? '').replace(/\D/g, '').replace(/^0+(?=\d)/, '');
        return digits.replace(/\B(?=(\d{3})+(?!\d))/g, '.');
    };

    const formatStoredValue = value => {
        const integerValue = String(value ?? '').trim().replace(/[.,]\d{1,2}$/, '');
        return formatDigits(integerValue);
    };

    document.addEventListener('DOMContentLoaded', () => {
        document.querySelectorAll('[data-currency-input]').forEach(input => {
            const hiddenInput = document.getElementById(input.dataset.currencyTarget);
            if (!hiddenInput) return;

            const updateValue = () => {
                const formattedValue = formatDigits(input.value);
                input.value = formattedValue;
                hiddenInput.value = formattedValue.replace(/\./g, '');
            };

            const initialValue = formatStoredValue(hiddenInput.value);
            input.value = input.dataset.emptyZero === 'true' && initialValue === '0'
                ? ''
                : initialValue;
            hiddenInput.value = input.value.replace(/\./g, '');

            input.addEventListener('input', updateValue);
        });
    });
})();
