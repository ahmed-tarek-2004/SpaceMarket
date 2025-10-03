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
    createService: '/Service/create-service',
    availableService: '/Service/client/available-service',
    availableDataset: '/Service/dataset/client/available',
    serviceDetail: '/Service/client/service-detail/',
    datasetDetails: '/Service/dataset/client/detail/',
  },
  serviceCategory: {
    getAllCategories: '/ServiceCategory',
  },
  cart: {
    addToCart: '/Cart/add-to-cart',
    cartContent: '/Cart/Cart/cart-content',
    updateQuantity: '/Cart/update-quantity',
    removeItem: `/Cart/remove/`,
    clearCart: '/Cart/clear-cart',
  },

  payment: {
    checkoutSession: '/Payment/checkout-session',
    success: '/Payment/success',
    cancel: '/Payment/cancel',
  },
  order: {
    createOrder: '/Order',
  },

  notification: {
    getAll: '/Notifications/user/notification',
    markAsRead: (id: string) => `/Notifications/user/mark-read/${id}`,
    getAllForAdmin: '/Notifications/admin/notification',
  },
};
