#ifdef SMPCORE_CPP

void SMPcore::initialize_opcode_table() {
  #define op opcode_table
  op[0x00] = &SMPcore::op_nop;
  op[0x01] = &SMPcore::op_tcall<0>;
  op[0x02] = &SMPcore::op_setbit_dp<1, 0x01>;
  op[0x03] = &SMPcore::op_bitbranch<0x01, true>;
  op[0x04] = &SMPcore::op_read_reg_dp<&SMPcore::op_or, A>;
  op[0x05] = &SMPcore::op_read_reg_addr<&SMPcore::op_or, A>;
  op[0x06] = &SMPcore::op_read_a_ix<&SMPcore::op_or>;
  op[0x07] = &SMPcore::op_read_a_idpx<&SMPcore::op_or>;
  op[0x08] = &SMPcore::op_read_reg_const<&SMPcore::op_or, A>;
  op[0x09] = &SMPcore::op_read_dp_dp<&SMPcore::op_or>;
  op[0x0a] = &SMPcore::op_or1_bit<0>;
  op[0x0b] = &SMPcore::op_adjust_dp<&SMPcore::op_asl>;
  op[0x0c] = &SMPcore::op_adjust_addr<&SMPcore::op_asl>;
  op[0x0d] = &SMPcore::op_push_p;
  op[0x0e] = &SMPcore::op_adjust_addr_a<1>;
  op[0x0f] = &SMPcore::op_brk;
  op[0x10] = &SMPcore::op_branch<0x80, false>;
  op[0x11] = &SMPcore::op_tcall<1>;
  op[0x12] = &SMPcore::op_setbit_dp<0, 0x01>;
  op[0x13] = &SMPcore::op_bitbranch<0x01, false>;
  op[0x14] = &SMPcore::op_read_a_dpx<&SMPcore::op_or>;
  op[0x15] = &SMPcore::op_read_a_addrr<&SMPcore::op_or, X>;
  op[0x16] = &SMPcore::op_read_a_addrr<&SMPcore::op_or, Y>;
  op[0x17] = &SMPcore::op_read_a_idpy<&SMPcore::op_or>;
  op[0x18] = &SMPcore::op_read_dp_const<&SMPcore::op_or>;
  op[0x19] = &SMPcore::op_read_ix_iy<&SMPcore::op_or>;
  op[0x1a] = &SMPcore::op_adjustw_dp<-1>;
  op[0x1b] = &SMPcore::op_adjust_dpx<&SMPcore::op_asl>;
  op[0x1c] = &SMPcore::op_adjust_reg<&SMPcore::op_asl, A>;
  op[0x1d] = &SMPcore::op_adjust_reg<&SMPcore::op_dec, X>;
  op[0x1e] = &SMPcore::op_read_reg_addr<&SMPcore::op_cmp, X>;
  op[0x1f] = &SMPcore::op_jmp_iaddrx;
  op[0x20] = &SMPcore::op_setbit<0x20, 0x00>;
  op[0x21] = &SMPcore::op_tcall<2>;
  op[0x22] = &SMPcore::op_setbit_dp<1, 0x02>;
  op[0x23] = &SMPcore::op_bitbranch<0x02, true>;
  op[0x24] = &SMPcore::op_read_reg_dp<&SMPcore::op_and, A>;
  op[0x25] = &SMPcore::op_read_reg_addr<&SMPcore::op_and, A>;
  op[0x26] = &SMPcore::op_read_a_ix<&SMPcore::op_and>;
  op[0x27] = &SMPcore::op_read_a_idpx<&SMPcore::op_and>;
  op[0x28] = &SMPcore::op_read_reg_const<&SMPcore::op_and, A>;
  op[0x29] = &SMPcore::op_read_dp_dp<&SMPcore::op_and>;
  op[0x2a] = &SMPcore::op_or1_bit<1>;
  op[0x2b] = &SMPcore::op_adjust_dp<&SMPcore::op_rol>;
  op[0x2c] = &SMPcore::op_adjust_addr<&SMPcore::op_rol>;
  op[0x2d] = &SMPcore::op_push_reg<A>;
  op[0x2e] = &SMPcore::op_cbne_dp;
  op[0x2f] = &SMPcore::op_bra;
  op[0x30] = &SMPcore::op_branch<0x80, true>;
  op[0x31] = &SMPcore::op_tcall<3>;
  op[0x32] = &SMPcore::op_setbit_dp<0, 0x02>;
  op[0x33] = &SMPcore::op_bitbranch<0x02, false>;
  op[0x34] = &SMPcore::op_read_a_dpx<&SMPcore::op_and>;
  op[0x35] = &SMPcore::op_read_a_addrr<&SMPcore::op_and, X>;
  op[0x36] = &SMPcore::op_read_a_addrr<&SMPcore::op_and, Y>;
  op[0x37] = &SMPcore::op_read_a_idpy<&SMPcore::op_and>;
  op[0x38] = &SMPcore::op_read_dp_const<&SMPcore::op_and>;
  op[0x39] = &SMPcore::op_read_ix_iy<&SMPcore::op_and>;
  op[0x3a] = &SMPcore::op_adjustw_dp<+1>;
  op[0x3b] = &SMPcore::op_adjust_dpx<&SMPcore::op_rol>;
  op[0x3c] = &SMPcore::op_adjust_reg<&SMPcore::op_rol, A>;
  op[0x3d] = &SMPcore::op_adjust_reg<&SMPcore::op_inc, X>;
  op[0x3e] = &SMPcore::op_read_reg_dp<&SMPcore::op_cmp, X>;
  op[0x3f] = &SMPcore::op_call;
  op[0x40] = &SMPcore::op_setbit<0x20, 0x20>;
  op[0x41] = &SMPcore::op_tcall<4>;
  op[0x42] = &SMPcore::op_setbit_dp<1, 0x04>;
  op[0x43] = &SMPcore::op_bitbranch<0x04, true>;
  op[0x44] = &SMPcore::op_read_reg_dp<&SMPcore::op_eor, A>;
  op[0x45] = &SMPcore::op_read_reg_addr<&SMPcore::op_eor, A>;
  op[0x46] = &SMPcore::op_read_a_ix<&SMPcore::op_eor>;
  op[0x47] = &SMPcore::op_read_a_idpx<&SMPcore::op_eor>;
  op[0x48] = &SMPcore::op_read_reg_const<&SMPcore::op_eor, A>;
  op[0x49] = &SMPcore::op_read_dp_dp<&SMPcore::op_eor>;
  op[0x4a] = &SMPcore::op_and1_bit<0>;
  op[0x4b] = &SMPcore::op_adjust_dp<&SMPcore::op_lsr>;
  op[0x4c] = &SMPcore::op_adjust_addr<&SMPcore::op_lsr>;
  op[0x4d] = &SMPcore::op_push_reg<X>;
  op[0x4e] = &SMPcore::op_adjust_addr_a<0>;
  op[0x4f] = &SMPcore::op_pcall;
  op[0x50] = &SMPcore::op_branch<0x40, false>;
  op[0x51] = &SMPcore::op_tcall<5>;
  op[0x52] = &SMPcore::op_setbit_dp<0, 0x04>;
  op[0x53] = &SMPcore::op_bitbranch<0x04, false>;
  op[0x54] = &SMPcore::op_read_a_dpx<&SMPcore::op_eor>;
  op[0x55] = &SMPcore::op_read_a_addrr<&SMPcore::op_eor, X>;
  op[0x56] = &SMPcore::op_read_a_addrr<&SMPcore::op_eor, Y>;
  op[0x57] = &SMPcore::op_read_a_idpy<&SMPcore::op_eor>;
  op[0x58] = &SMPcore::op_read_dp_const<&SMPcore::op_eor>;
  op[0x59] = &SMPcore::op_read_ix_iy<&SMPcore::op_eor>;
  op[0x5a] = &SMPcore::op_cmpw_ya_dp;
  op[0x5b] = &SMPcore::op_adjust_dpx<&SMPcore::op_lsr>;
  op[0x5c] = &SMPcore::op_adjust_reg<&SMPcore::op_lsr, A>;
  op[0x5d] = &SMPcore::op_mov_reg_reg<X, A>;
  op[0x5e] = &SMPcore::op_read_reg_addr<&SMPcore::op_cmp, Y>;
  op[0x5f] = &SMPcore::op_jmp_addr;
  op[0x60] = &SMPcore::op_setbit<0x01, 0x00>;
  op[0x61] = &SMPcore::op_tcall<6>;
  op[0x62] = &SMPcore::op_setbit_dp<1, 0x08>;
  op[0x63] = &SMPcore::op_bitbranch<0x08, true>;
  op[0x64] = &SMPcore::op_read_reg_dp<&SMPcore::op_cmp, A>;
  op[0x65] = &SMPcore::op_read_reg_addr<&SMPcore::op_cmp, A>;
  op[0x66] = &SMPcore::op_read_a_ix<&SMPcore::op_cmp>;
  op[0x67] = &SMPcore::op_read_a_idpx<&SMPcore::op_cmp>;
  op[0x68] = &SMPcore::op_read_reg_const<&SMPcore::op_cmp, A>;
  op[0x69] = &SMPcore::op_read_dp_dp<&SMPcore::op_cmp>;
  op[0x6a] = &SMPcore::op_and1_bit<1>;
  op[0x6b] = &SMPcore::op_adjust_dp<&SMPcore::op_ror>;
  op[0x6c] = &SMPcore::op_adjust_addr<&SMPcore::op_ror>;
  op[0x6d] = &SMPcore::op_push_reg<Y>;
  op[0x6e] = &SMPcore::op_dbnz_dp;
  op[0x6f] = &SMPcore::op_ret;
  op[0x70] = &SMPcore::op_branch<0x40, true>;
  op[0x71] = &SMPcore::op_tcall<7>;
  op[0x72] = &SMPcore::op_setbit_dp<0, 0x08>;
  op[0x73] = &SMPcore::op_bitbranch<0x08, false>;
  op[0x74] = &SMPcore::op_read_a_dpx<&SMPcore::op_cmp>;
  op[0x75] = &SMPcore::op_read_a_addrr<&SMPcore::op_cmp, X>;
  op[0x76] = &SMPcore::op_read_a_addrr<&SMPcore::op_cmp, Y>;
  op[0x77] = &SMPcore::op_read_a_idpy<&SMPcore::op_cmp>;
  op[0x78] = &SMPcore::op_read_dp_const<&SMPcore::op_cmp>;
  op[0x79] = &SMPcore::op_read_ix_iy<&SMPcore::op_cmp>;
  op[0x7a] = &SMPcore::op_read_ya_dp<&SMPcore::op_addw>;
  op[0x7b] = &SMPcore::op_adjust_dpx<&SMPcore::op_ror>;
  op[0x7c] = &SMPcore::op_adjust_reg<&SMPcore::op_ror, A>;
  op[0x7d] = &SMPcore::op_mov_reg_reg<A, X>;
  op[0x7e] = &SMPcore::op_read_reg_dp<&SMPcore::op_cmp, Y>;
  op[0x7f] = &SMPcore::op_reti;
  op[0x80] = &SMPcore::op_setbit<0x01, 0x01>;
  op[0x81] = &SMPcore::op_tcall<8>;
  op[0x82] = &SMPcore::op_setbit_dp<1, 0x10>;
  op[0x83] = &SMPcore::op_bitbranch<0x10, true>;
  op[0x84] = &SMPcore::op_read_reg_dp<&SMPcore::op_adc, A>;
  op[0x85] = &SMPcore::op_read_reg_addr<&SMPcore::op_adc, A>;
  op[0x86] = &SMPcore::op_read_a_ix<&SMPcore::op_adc>;
  op[0x87] = &SMPcore::op_read_a_idpx<&SMPcore::op_adc>;
  op[0x88] = &SMPcore::op_read_reg_const<&SMPcore::op_adc, A>;
  op[0x89] = &SMPcore::op_read_dp_dp<&SMPcore::op_adc>;
  op[0x8a] = &SMPcore::op_eor1_bit;
  op[0x8b] = &SMPcore::op_adjust_dp<&SMPcore::op_dec>;
  op[0x8c] = &SMPcore::op_adjust_addr<&SMPcore::op_dec>;
  op[0x8d] = &SMPcore::op_mov_reg_const<Y>;
  op[0x8e] = &SMPcore::op_pop_p;
  op[0x8f] = &SMPcore::op_mov_dp_const;
  op[0x90] = &SMPcore::op_branch<0x01, false>;
  op[0x91] = &SMPcore::op_tcall<9>;
  op[0x92] = &SMPcore::op_setbit_dp<0, 0x10>;
  op[0x93] = &SMPcore::op_bitbranch<0x10, false>;
  op[0x94] = &SMPcore::op_read_a_dpx<&SMPcore::op_adc>;
  op[0x95] = &SMPcore::op_read_a_addrr<&SMPcore::op_adc, X>;
  op[0x96] = &SMPcore::op_read_a_addrr<&SMPcore::op_adc, Y>;
  op[0x97] = &SMPcore::op_read_a_idpy<&SMPcore::op_adc>;
  op[0x98] = &SMPcore::op_read_dp_const<&SMPcore::op_adc>;
  op[0x99] = &SMPcore::op_read_ix_iy<&SMPcore::op_adc>;
  op[0x9a] = &SMPcore::op_read_ya_dp<&SMPcore::op_subw>;
  op[0x9b] = &SMPcore::op_adjust_dpx<&SMPcore::op_dec>;
  op[0x9c] = &SMPcore::op_adjust_reg<&SMPcore::op_dec, A>;
  op[0x9d] = &SMPcore::op_mov_reg_reg<X, SP>;
  op[0x9e] = &SMPcore::op_div_ya_x;
  op[0x9f] = &SMPcore::op_xcn;
  op[0xa0] = &SMPcore::op_seti<1>;
  op[0xa1] = &SMPcore::op_tcall<10>;
  op[0xa2] = &SMPcore::op_setbit_dp<1, 0x20>;
  op[0xa3] = &SMPcore::op_bitbranch<0x20, true>;
  op[0xa4] = &SMPcore::op_read_reg_dp<&SMPcore::op_sbc, A>;
  op[0xa5] = &SMPcore::op_read_reg_addr<&SMPcore::op_sbc, A>;
  op[0xa6] = &SMPcore::op_read_a_ix<&SMPcore::op_sbc>;
  op[0xa7] = &SMPcore::op_read_a_idpx<&SMPcore::op_sbc>;
  op[0xa8] = &SMPcore::op_read_reg_const<&SMPcore::op_sbc, A>;
  op[0xa9] = &SMPcore::op_read_dp_dp<&SMPcore::op_sbc>;
  op[0xaa] = &SMPcore::op_mov1_c_bit;
  op[0xab] = &SMPcore::op_adjust_dp<&SMPcore::op_inc>;
  op[0xac] = &SMPcore::op_adjust_addr<&SMPcore::op_inc>;
  op[0xad] = &SMPcore::op_read_reg_const<&SMPcore::op_cmp, Y>;
  op[0xae] = &SMPcore::op_pop_reg<A>;
  op[0xaf] = &SMPcore::op_mov_ixinc_a;
  op[0xb0] = &SMPcore::op_branch<0x01, true>;
  op[0xb1] = &SMPcore::op_tcall<11>;
  op[0xb2] = &SMPcore::op_setbit_dp<0, 0x20>;
  op[0xb3] = &SMPcore::op_bitbranch<0x20, false>;
  op[0xb4] = &SMPcore::op_read_a_dpx<&SMPcore::op_sbc>;
  op[0xb5] = &SMPcore::op_read_a_addrr<&SMPcore::op_sbc, X>;
  op[0xb6] = &SMPcore::op_read_a_addrr<&SMPcore::op_sbc, Y>;
  op[0xb7] = &SMPcore::op_read_a_idpy<&SMPcore::op_sbc>;
  op[0xb8] = &SMPcore::op_read_dp_const<&SMPcore::op_sbc>;
  op[0xb9] = &SMPcore::op_read_ix_iy<&SMPcore::op_sbc>;
  op[0xba] = &SMPcore::op_movw_ya_dp;
  op[0xbb] = &SMPcore::op_adjust_dpx<&SMPcore::op_inc>;
  op[0xbc] = &SMPcore::op_adjust_reg<&SMPcore::op_inc, A>;
  op[0xbd] = &SMPcore::op_mov_sp_x;
  op[0xbe] = &SMPcore::op_das;
  op[0xbf] = &SMPcore::op_mov_a_ixinc;
  op[0xc0] = &SMPcore::op_seti<0>;
  op[0xc1] = &SMPcore::op_tcall<12>;
  op[0xc2] = &SMPcore::op_setbit_dp<1, 0x40>;
  op[0xc3] = &SMPcore::op_bitbranch<0x40, true>;
  op[0xc4] = &SMPcore::op_mov_dp_reg<A>;
  op[0xc5] = &SMPcore::op_mov_addr_reg<A>;
  op[0xc6] = &SMPcore::op_mov_ix_a;
  op[0xc7] = &SMPcore::op_mov_idpx_a;
  op[0xc8] = &SMPcore::op_read_reg_const<&SMPcore::op_cmp, X>;
  op[0xc9] = &SMPcore::op_mov_addr_reg<X>;
  op[0xca] = &SMPcore::op_mov1_bit_c;
  op[0xcb] = &SMPcore::op_mov_dp_reg<Y>;
  op[0xcc] = &SMPcore::op_mov_addr_reg<Y>;
  op[0xcd] = &SMPcore::op_mov_reg_const<X>;
  op[0xce] = &SMPcore::op_pop_reg<X>;
  op[0xcf] = &SMPcore::op_mul_ya;
  op[0xd0] = &SMPcore::op_branch<0x02, false>;
  op[0xd1] = &SMPcore::op_tcall<13>;
  op[0xd2] = &SMPcore::op_setbit_dp<0, 0x40>;
  op[0xd3] = &SMPcore::op_bitbranch<0x40, false>;
  op[0xd4] = &SMPcore::op_mov_dpr_reg<A, X>;
  op[0xd5] = &SMPcore::op_mov_addrr_a<X>;
  op[0xd6] = &SMPcore::op_mov_addrr_a<Y>;
  op[0xd7] = &SMPcore::op_mov_idpy_a;
  op[0xd8] = &SMPcore::op_mov_dp_reg<X>;
  op[0xd9] = &SMPcore::op_mov_dpr_reg<X, Y>;
  op[0xda] = &SMPcore::op_movw_dp_ya;
  op[0xdb] = &SMPcore::op_mov_dpr_reg<Y, X>;
  op[0xdc] = &SMPcore::op_adjust_reg<&SMPcore::op_dec, Y>;
  op[0xdd] = &SMPcore::op_mov_reg_reg<A, Y>;
  op[0xde] = &SMPcore::op_cbne_dpx;
  op[0xdf] = &SMPcore::op_daa;
  op[0xe0] = &SMPcore::op_setbit<0x48, 0x00>;
  op[0xe1] = &SMPcore::op_tcall<14>;
  op[0xe2] = &SMPcore::op_setbit_dp<1, 0x80>;
  op[0xe3] = &SMPcore::op_bitbranch<0x80, true>;
  op[0xe4] = &SMPcore::op_mov_reg_dp<A>;
  op[0xe5] = &SMPcore::op_mov_reg_addr<A>;
  op[0xe6] = &SMPcore::op_mov_a_ix;
  op[0xe7] = &SMPcore::op_mov_a_idpx;
  op[0xe8] = &SMPcore::op_mov_reg_const<A>;
  op[0xe9] = &SMPcore::op_mov_reg_addr<X>;
  op[0xea] = &SMPcore::op_not1_bit;
  op[0xeb] = &SMPcore::op_mov_reg_dp<Y>;
  op[0xec] = &SMPcore::op_mov_reg_addr<Y>;
  op[0xed] = &SMPcore::op_notc;
  op[0xee] = &SMPcore::op_pop_reg<Y>;
  op[0xef] = &SMPcore::op_wait;
  op[0xf0] = &SMPcore::op_branch<0x02, true>;
  op[0xf1] = &SMPcore::op_tcall<15>;
  op[0xf2] = &SMPcore::op_setbit_dp<0, 0x80>;
  op[0xf3] = &SMPcore::op_bitbranch<0x80, false>;
  op[0xf4] = &SMPcore::op_mov_reg_dpr<A, X>;
  op[0xf5] = &SMPcore::op_mov_a_addrr<X>;
  op[0xf6] = &SMPcore::op_mov_a_addrr<Y>;
  op[0xf7] = &SMPcore::op_mov_a_idpy;
  op[0xf8] = &SMPcore::op_mov_reg_dp<X>;
  op[0xf9] = &SMPcore::op_mov_reg_dpr<X, Y>;
  op[0xfa] = &SMPcore::op_mov_dp_dp;
  op[0xfb] = &SMPcore::op_mov_reg_dpr<Y, X>;
  op[0xfc] = &SMPcore::op_adjust_reg<&SMPcore::op_inc, Y>;
  op[0xfd] = &SMPcore::op_mov_reg_reg<Y, A>;
  op[0xfe] = &SMPcore::op_dbnz_y;
  op[0xff] = &SMPcore::op_wait;
  #undef op
}

#endif
