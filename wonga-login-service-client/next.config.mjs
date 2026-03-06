/** @type {import('next').NextConfig} */
const nextConfig = {
  output: 'standalone',
  images: {
    remotePatterns: [
      {
        protocol: 'https',
        hostname: 'www.liblogo.com',
        pathname: '/img-logo/**',
      },
    ],
  },
  eslint: {
    // Run ESLint on these directories during builds
    dirs: ['pages', 'components', 'lib', 'app', 'src'],
  },
  typescript: {
    // Type checking is already done by `tsc` in CI/CD
    // Set to true if you want to ignore type errors during builds
    ignoreBuildErrors: false,
  },
};

export default nextConfig;