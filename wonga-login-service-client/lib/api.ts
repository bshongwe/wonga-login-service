import {
  LoginFormData,
  RegisterFormData,
} from "./schemas";

const API_URL =
  process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000/api";

/**
 * Validate endpoint to prevent SSRF attacks
 */
function validateEndpoint(endpoint: string): void {
  if (!endpoint.startsWith("/")) {
    throw new Error("Invalid endpoint: must start with /");
  }
  if (endpoint.includes("://") || endpoint.includes("//")) {
    throw new Error("Invalid endpoint: protocol or double slashes not allowed");
  }
}

/**
 * Generic API error class
 */
export class ApiError extends Error {
  constructor(
    public status: number,
    message: string
  ) {
    super(message);
    this.name = "ApiError";
  }
}

/**
 * Generic fetch wrapper with error handling
 * Tokens are sent via httpOnly cookies automatically
 */
async function fetchApi<T>(
  endpoint: string,
  options?: RequestInit
): Promise<T> {
  validateEndpoint(endpoint);
  
  const response = await fetch(`${API_URL}${endpoint}`, {
    headers: {
      "Content-Type": "application/json",
      ...options?.headers,
    },
    credentials: "include", // Send httpOnly cookies
    ...options,
  });

  if (!response.ok) {
    const error = await response
      .json()
      .catch(() => ({ message: "An error occurred" }));
    throw new ApiError(
      response.status,
      error.message || "Request failed"
    );
  }

  return response.json();
}

/**
 * User authentication API
 * All requests proxy through Next.js API routes to set httpOnly cookies
 */
export const authApi = {
  /**
   * Login user
   */
  login: async (data: LoginFormData) => {
    const response = await fetch("/api/auth/login", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data),
      credentials: "include",
    });

    if (!response.ok) {
      const error = await response.json().catch(() => ({ message: "Login failed" }));
      throw new ApiError(response.status, error.message || "Login failed");
    }

    return response.json();
  },

  /**
   * Register new user
   */
  register: async (data: RegisterFormData) => {
    const response = await fetch("/api/auth/register", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data),
      credentials: "include",
    });

    if (!response.ok) {
      const error = await response.json().catch(() => ({ message: "Registration failed" }));
      throw new ApiError(response.status, error.message || "Registration failed");
    }

    return response.json();
  },

  /**
   * Get current user details
   * Token sent automatically via httpOnly cookie
   */
  getUserDetails: async () => {
    return fetchApi<UserResponse>("/user/details");
  },
};

/**
 * User response type
 */
export interface UserResponse {
  id: string;
  username: string;
  email: string;
  createdAt: string;
}
