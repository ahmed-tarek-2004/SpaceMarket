export const environment = {
  production: false,

  apiUrl: 'https://spacemarket.runasp.net/api',
  // apiUrl: 'https://localhost:7299/api',

  account: {
    signIn: '/Account/login',
    clientSignup: '/Account/register/client',
    providerSignup: '/Account/register/provider',
    verifyOtp: '/Account/verify-otp',
    forgetPassword: '/Account/forget-password',
    resetPassword: '/Account/reset-password',
    refresh: '/Account/refresh-token',
  },
  service: {
    createService: '/Service/create',
    availaleService: '/Service/client/available-service',
  },
  serviceCategory: {
    getAllCategories: '/ServiceCategory',
  },
  cart: {
    cartContent: '/Cart/Cart/cart-content',
    updateQuantity: '/Cart/update-quantity',
    removeItem: `/Cart/remove/`,
    clearCart: '/Cart/clear-cart',
  },
};
