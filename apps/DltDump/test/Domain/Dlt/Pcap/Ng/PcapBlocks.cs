﻿namespace RJCP.App.DltDump.Domain.Dlt.Pcap.Ng
{
    public static class PcapBlocks
    {
        public static readonly byte[] ShbData = new byte[] {
            0x0A, 0x0D, 0x0D, 0x0A, 0xC0, 0x00, 0x00, 0x00, 0x4D, 0x3C, 0x2B, 0x1A, 0x01, 0x00, 0x00, 0x00,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x02, 0x00, 0x36, 0x00, 0x49, 0x6E, 0x74, 0x65,
            0x6C, 0x28, 0x52, 0x29, 0x20, 0x43, 0x6F, 0x72, 0x65, 0x28, 0x54, 0x4D, 0x29, 0x20, 0x69, 0x37,
            0x2D, 0x36, 0x37, 0x30, 0x30, 0x54, 0x20, 0x43, 0x50, 0x55, 0x20, 0x40, 0x20, 0x32, 0x2E, 0x38,
            0x30, 0x47, 0x48, 0x7A, 0x20, 0x28, 0x77, 0x69, 0x74, 0x68, 0x20, 0x53, 0x53, 0x45, 0x34, 0x2E,
            0x32, 0x29, 0x00, 0x00, 0x03, 0x00, 0x25, 0x00, 0x36, 0x34, 0x2D, 0x62, 0x69, 0x74, 0x20, 0x57,
            0x69, 0x6E, 0x64, 0x6F, 0x77, 0x73, 0x20, 0x31, 0x30, 0x20, 0x28, 0x32, 0x30, 0x30, 0x39, 0x29,
            0x2C, 0x20, 0x62, 0x75, 0x69, 0x6C, 0x64, 0x20, 0x31, 0x39, 0x30, 0x34, 0x33, 0x00, 0x00, 0x00,
            0x04, 0x00, 0x32, 0x00, 0x44, 0x75, 0x6D, 0x70, 0x63, 0x61, 0x70, 0x20, 0x28, 0x57, 0x69, 0x72,
            0x65, 0x73, 0x68, 0x61, 0x72, 0x6B, 0x29, 0x20, 0x33, 0x2E, 0x34, 0x2E, 0x33, 0x20, 0x28, 0x76,
            0x33, 0x2E, 0x34, 0x2E, 0x33, 0x2D, 0x30, 0x2D, 0x67, 0x36, 0x61, 0x65, 0x36, 0x63, 0x64, 0x33,
            0x33, 0x35, 0x61, 0x61, 0x39, 0x29, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x00, 0x00, 0x00
        };

        public static readonly byte[] ShbDataBigEndian = new byte[] {
            0x0A, 0x0D, 0x0D, 0x0A, 0x00, 0x00, 0x00, 0xC0, 0x1A, 0x2B, 0x3C, 0x4D, 0x00, 0x01, 0x00, 0x00,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x02, 0x00, 0x36, 0x49, 0x6E, 0x74, 0x65,
            0x6C, 0x28, 0x52, 0x29, 0x20, 0x43, 0x6F, 0x72, 0x65, 0x28, 0x54, 0x4D, 0x29, 0x20, 0x69, 0x37,
            0x2D, 0x36, 0x37, 0x30, 0x30, 0x54, 0x20, 0x43, 0x50, 0x55, 0x20, 0x40, 0x20, 0x32, 0x2E, 0x38,
            0x30, 0x47, 0x48, 0x7A, 0x20, 0x28, 0x77, 0x69, 0x74, 0x68, 0x20, 0x53, 0x53, 0x45, 0x34, 0x2E,
            0x32, 0x29, 0x00, 0x00, 0x00, 0x03, 0x00, 0x25, 0x36, 0x34, 0x2D, 0x62, 0x69, 0x74, 0x20, 0x57,
            0x69, 0x6E, 0x64, 0x6F, 0x77, 0x73, 0x20, 0x31, 0x30, 0x20, 0x28, 0x32, 0x30, 0x30, 0x39, 0x29,
            0x2C, 0x20, 0x62, 0x75, 0x69, 0x6C, 0x64, 0x20, 0x31, 0x39, 0x30, 0x34, 0x33, 0x00, 0x00, 0x00,
            0x00, 0x04, 0x00, 0x32, 0x44, 0x75, 0x6D, 0x70, 0x63, 0x61, 0x70, 0x20, 0x28, 0x57, 0x69, 0x72,
            0x65, 0x73, 0x68, 0x61, 0x72, 0x6B, 0x29, 0x20, 0x33, 0x2E, 0x34, 0x2E, 0x33, 0x20, 0x28, 0x76,
            0x33, 0x2E, 0x34, 0x2E, 0x33, 0x2D, 0x30, 0x2D, 0x67, 0x36, 0x61, 0x65, 0x36, 0x63, 0x64, 0x33,
            0x33, 0x35, 0x61, 0x61, 0x39, 0x29, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0
        };

        public static readonly byte[] ShbSmall = new byte[] {
            0x0A, 0x0D, 0x0D, 0x0A, 0x1C, 0x00, 0x00, 0x00, 0x4D, 0x3C, 0x2B, 0x1A, 0x01, 0x00, 0x00, 0x00,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x1C, 0x00, 0x00, 0x00
        };

        public static readonly byte[] ShbSmallBigEndian = new byte[] {
            0x0A, 0x0D, 0x0D, 0x0A, 0x00, 0x00, 0x00, 0x1C, 0x1A, 0x2B, 0x3C, 0x4D, 0x00, 0x01, 0x00, 0x00,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x1C
        };

        public static readonly byte[] IdbData = new byte[] {
            0x01, 0x00, 0x00, 0x00, 0x90, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00,
            0x02, 0x00, 0x32, 0x00, 0x5C, 0x44, 0x65, 0x76, 0x69, 0x63, 0x65, 0x5C, 0x4E, 0x50, 0x46, 0x5F,
            0x7B, 0x32, 0x38, 0x46, 0x45, 0x37, 0x41, 0x45, 0x32, 0x2D, 0x35, 0x38, 0x38, 0x34, 0x2D, 0x34,
            0x31, 0x38, 0x32, 0x2D, 0x39, 0x32, 0x43, 0x41, 0x2D, 0x35, 0x33, 0x31, 0x41, 0x36, 0x46, 0x33,
            0x37, 0x37, 0x38, 0x37, 0x36, 0x7D, 0x00, 0x00, 0x03, 0x00, 0x08, 0x00, 0x45, 0x74, 0x68, 0x65,
            0x72, 0x6E, 0x65, 0x74, 0x09, 0x00, 0x01, 0x00, 0x06, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x25, 0x00,
            0x36, 0x34, 0x2D, 0x62, 0x69, 0x74, 0x20, 0x57, 0x69, 0x6E, 0x64, 0x6F, 0x77, 0x73, 0x20, 0x31,
            0x30, 0x20, 0x28, 0x32, 0x30, 0x30, 0x39, 0x29, 0x2C, 0x20, 0x62, 0x75, 0x69, 0x6C, 0x64, 0x20,
            0x31, 0x39, 0x30, 0x34, 0x33, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x90, 0x00, 0x00, 0x00
        };

        public static readonly byte[] IdbDataBigEndian = new byte[] {
            0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x90, 0x00, 0x01, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00,
            0x00, 0x02, 0x00, 0x32, 0x5C, 0x44, 0x65, 0x76, 0x69, 0x63, 0x65, 0x5C, 0x4E, 0x50, 0x46, 0x5F,
            0x7B, 0x32, 0x38, 0x46, 0x45, 0x37, 0x41, 0x45, 0x32, 0x2D, 0x35, 0x38, 0x38, 0x34, 0x2D, 0x34,
            0x31, 0x38, 0x32, 0x2D, 0x39, 0x32, 0x43, 0x41, 0x2D, 0x35, 0x33, 0x31, 0x41, 0x36, 0x46, 0x33,
            0x37, 0x37, 0x38, 0x37, 0x36, 0x7D, 0x00, 0x00, 0x00, 0x03, 0x00, 0x08, 0x45, 0x74, 0x68, 0x65,
            0x72, 0x6E, 0x65, 0x74, 0x00, 0x09, 0x00, 0x01, 0x06, 0x00, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x25,
            0x36, 0x34, 0x2D, 0x62, 0x69, 0x74, 0x20, 0x57, 0x69, 0x6E, 0x64, 0x6F, 0x77, 0x73, 0x20, 0x31,
            0x30, 0x20, 0x28, 0x32, 0x30, 0x30, 0x39, 0x29, 0x2C, 0x20, 0x62, 0x75, 0x69, 0x6C, 0x64, 0x20,
            0x31, 0x39, 0x30, 0x34, 0x33, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x90
        };

        public static readonly byte[] IdbSmallLinkEth = new byte[] {
            0x01, 0x00, 0x00, 0x00, 0x14, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00,
            0x14, 0x00, 0x00, 0x00
        };

        public static readonly byte[] IdbSmallLinkEthBigEndian = new byte[] {
            0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x14, 0x00, 0x01, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x14
        };

        public static readonly byte[] IdbSmallLinkSll = new byte[] {
            0x01, 0x00, 0x00, 0x00, 0x14, 0x00, 0x00, 0x00, 0x71, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00,
            0x14, 0x00, 0x00, 0x00
        };

        public static readonly byte[] IdbSmallLinkSllBigEndian = new byte[] {
            0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x14, 0x00, 0x71, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x14
        };

        public static readonly byte[] IdbEthMicroSec = new byte[] {
            0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00,
            0x09, 0x00, 0x01, 0x00, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00
        };

        public static readonly byte[] IdbEthMicroSecBigEndian = new byte[] {
            0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x01, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00,
            0x00, 0x09, 0x00, 0x01, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20
        };

        public static readonly byte[] IdbEthMicroSecNoEOpt = new byte[] {
            0x01, 0x00, 0x00, 0x00, 0x1C, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00,
            0x09, 0x00, 0x01, 0x00, 0x06, 0x00, 0x00, 0x00, 0x1C, 0x00, 0x00, 0x00
        };

        public static readonly byte[] IdbEthMicroSecNoEOptBigEndian = new byte[] {
            0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x1C, 0x00, 0x01, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00,
            0x00, 0x09, 0x00, 0x01, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1C
        };

        public static readonly byte[] IdbEthNanoSec = new byte[] {
            0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00,
            0x09, 0x00, 0x01, 0x00, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00
        };

        public static readonly byte[] IdbEthNanoSecBigEndian = new byte[] {
            0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x01, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00,
            0x00, 0x09, 0x00, 0x01, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20
        };

        public static readonly byte[] CustomSmall = new byte[] {
            0x20, 0x00, 0x00, 0x00, 0x1C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x1C, 0x00, 0x00, 0x00
        };

        public static readonly byte[] CustomSmallBigEndian = new byte[] {
            0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x1C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x1C
        };
    }
}
